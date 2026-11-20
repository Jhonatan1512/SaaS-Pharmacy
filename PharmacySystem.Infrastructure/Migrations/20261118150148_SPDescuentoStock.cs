using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SPDescuentoStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHBOG4SMi9wY/5M2In4J37FvkER/JZAwkRMZrO3VOcENe+V6VXivW8gO2zwqSt/zpw==");

            var sql = @"
                CREATE OR ALTER PROCEDURE sp_DescontarStockVenta
                    @InventarioSedeId UNIQUEIDENTIFIER,
                    @SedeId UNIQUEIDENTIFIER,
                    @CantidadUnidadesMinimas INT,
                    @EstadoActivo INT,
                    @EstadoAgotado INT,
                    @FechaActual DATE 
                AS
                BEGIN
                    SET NOCOUNT ON;

                    IF @CantidadUnidadesMinimas <= 0
                    BEGIN
                        THROW 50001, 'La cantidad a descontar debe ser mayor a cero.', 1;
                    END

                    DECLARE @StockActual INT;
                    DECLARE @SedeRegistro UNIQUEIDENTIFIER;
                    DECLARE @EstadoActual INT;
                    DECLARE @ProductoId UNIQUEIDENTIFIER;
                    DECLARE @FechaVencimiento DATE;
                    DECLARE @NumeroLote NVARCHAR(100);
                    DECLARE @NombreProducto NVARCHAR(200);

                    -- Bloqueo a nivel de fila para evitar sobreventa concurrente
                    SELECT 
                        @StockActual = i.StockFisicoUnidades,
                        @SedeRegistro = i.SedeId,
                        @EstadoActual = i.Estado,
                        @ProductoId = l.ProductoId,
                        @FechaVencimiento = CAST(l.FechaVencimiento AS DATE),
                        @NumeroLote = l.CodigoLote,
                        @NombreProducto = p.NombreComercial
                    FROM InventarioSedes i WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN Lotes l ON i.LoteId = l.Id
                    INNER JOIN Productos p ON l.ProductoId = p.Id
                    WHERE i.Id = @InventarioSedeId;

                    IF @StockActual IS NULL
                    BEGIN
                        THROW 50002, 'El registro de inventario especificado no existe.', 1;
                    END

                    IF @SedeRegistro <> @SedeId
                    BEGIN
                        THROW 50003, 'El producto seleccionado no pertenece al inventario de su sede.', 1;
                    END

                    IF @EstadoActual <> @EstadoActivo
                    BEGIN
                        DECLARE @MsgEstado NVARCHAR(250) = CONCAT('El lote ', @NumeroLote, ' de ', @NombreProducto, ' no se encuentra activo para la venta.');
                        THROW 50004, @MsgEstado, 1;
                    END

                    IF @FechaVencimiento < @FechaActual
                    BEGIN
                        DECLARE @MsgVencido NVARCHAR(250) = CONCAT('El lote ', @NumeroLote, ' de ', @NombreProducto, ' se encuentra vencido.');
                        THROW 50005, @MsgVencido, 1;
                    END

                    --    del mismo producto en esta sede cuya fecha de vencimiento sea anterior
                    DECLARE @LotePrioritario NVARCHAR(100);
                    DECLARE @VencimientoPrioritario VARCHAR(10);

                    SELECT TOP 1
                        @LotePrioritario = l2.CodigoLote,
                        @VencimientoPrioritario = CONVERT(VARCHAR(10), l2.FechaVencimiento, 103)
                    FROM InventarioSedes i2 WITH (UPDLOCK)
                    INNER JOIN Lotes l2 ON i2.LoteId = l2.Id
                    WHERE i2.SedeId = @SedeId
                      AND l2.ProductoId = @ProductoId
                      AND i2.Id <> @InventarioSedeId
                      AND i2.Estado = @EstadoActivo
                      AND i2.StockFisicoUnidades > 0
                      AND CAST(l2.FechaVencimiento AS DATE) >= @FechaActual
                      AND CAST(l2.FechaVencimiento AS DATE) < @FechaVencimiento
                    ORDER BY l2.FechaVencimiento ASC;

                    IF @LotePrioritario IS NOT NULL
                    BEGIN
                        DECLARE @MsgFefo NVARCHAR(350) = CONCAT(
                            'Restricción FEFO: Debe vender primero el lote ', @LotePrioritario,
                            ' de ', @NombreProducto, ' (vence el ', @VencimientoPrioritario, ').'
                        );
                        THROW 50007, @MsgFefo, 1;
                    END
                        
                    -- Validar stock disponible
                    IF @StockActual < @CantidadUnidadesMinimas
                    BEGIN
                        DECLARE @MsgStock NVARCHAR(300) = CONCAT('Stock insuficiente para ', @NombreProducto, ' (Lote ', @NumeroLote, '). Disponible: ', @StockActual, ', solicitado: ', @CantidadUnidadesMinimas, '.');
                        THROW 50006, @MsgStock, 1;
                    END

                    -- Descuento de stock y cambio automático a Agotado si llega a 0
                    UPDATE InventarioSedes
                    SET 
                        StockFisicoUnidades = StockFisicoUnidades - @CantidadUnidadesMinimas,
                        Estado = CASE 
                                    WHEN (StockFisicoUnidades - @CantidadUnidadesMinimas) = 0 THEN @EstadoAgotado 
                                    ELSE @EstadoActivo 
                                 END
                    WHERE Id = @InventarioSedeId;
                END;";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DescontarStockVenta;");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ3GeVmV9n1dvtQiAb3Wkw9g8QWhr4z6+qCOtqapzdvKlwB7H4o0RsaIaxNzGUkUvA==");
        }
    }
}