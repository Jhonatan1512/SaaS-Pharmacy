using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionProcesoVenta_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHqwUNhW9vXOAi+fb61xHhBPEyc6a9oVM6uXclgHWOIrARnsb5TUp3AW0Ug7+sbdKQ==");

            var spPrepararVenta = @"
                CREATE OR ALTER PROCEDURE sp_PrepararCabeceraVenta
                    @SedeId UNIQUEIDENTIFIER,
                    @UsuarioId UNIQUEIDENTIFIER,
                    @TurnoCajaId UNIQUEIDENTIFIER,
                    @TipoComprobante INT,
                    @SerieOut NVARCHAR(20) OUTPUT,
                    @NumeroOut NVARCHAR(20) OUTPUT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    -- 1. Validar que la Sede exista y esté activa
                    DECLARE @SedeActiva BIT;
                    SELECT @SedeActiva = Activa FROM Sedes WHERE Id = @SedeId;

                    IF @SedeActiva IS NULL
                        THROW 50030, 'La sede especificada no existe.', 1;

                    IF @SedeActiva = 0
                        THROW 50031, 'No se pueden registrar ventas porque la sede se encuentra inactiva.', 1;

                    -- 2. Validar Turno de Caja abierto en BD (evita ventas si el turno se cerró concurrentemente)
                    DECLARE @TurnoAbierto BIT;
                    DECLARE @TurnoSedeId UNIQUEIDENTIFIER;
                    DECLARE @TurnoUsuarioId UNIQUEIDENTIFIER;

                    SELECT 
                        @TurnoAbierto = EstadoAbierto,
                        @TurnoSedeId = SedeId,
                        @TurnoUsuarioId = UsuarioId
                    FROM TurnosCaja WITH (UPDLOCK, ROWLOCK)
                    WHERE Id = @TurnoCajaId;

                    IF @TurnoAbierto IS NULL
                        THROW 50032, 'El turno de caja especificado no existe.', 1;

                    IF @TurnoSedeId <> @SedeId OR @TurnoUsuarioId <> @UsuarioId
                        THROW 50033, 'El turno de caja no pertenece al farmacéutico o a la sede actual.', 1;

                    IF @TurnoAbierto = 0
                        THROW 50034, 'El turno de caja ya se encuentra cerrado. Debe abrir un nuevo turno para vender.', 1;

                    -- 3. Bloquear serie activa y avanzar correlativo de forma atómica (sin colisión entre cajeros)
                    DECLARE @SerieId UNIQUEIDENTIFIER;
                    DECLARE @NuevoCorrelativo INT;

                    SELECT TOP 1
                        @SerieId = Id,
                        @SerieOut = Serie,
                        @NuevoCorrelativo = UltimoCorrelativo + 1
                    FROM SeriesComprobantes WITH (UPDLOCK, ROWLOCK)
                    WHERE SedeId = @SedeId
                      AND TipoComprobante = @TipoComprobante
                      AND Activo = 1;

                    IF @SerieId IS NULL
                        THROW 50035, 'No existe una serie activa configurada para este tipo de comprobante en la sede.', 1;

                    UPDATE SeriesComprobantes
                    SET UltimoCorrelativo = @NuevoCorrelativo
                    WHERE Id = @SerieId;

                    SET @NumeroOut = RIGHT(REPLICATE('0', 8) + CAST(@NuevoCorrelativo AS VARCHAR(20)), 8);
                END;";

            var spDescontarStock = @"
                CREATE OR ALTER PROCEDURE sp_DescontarStockVenta
                    @InventarioSedeId UNIQUEIDENTIFIER,
                    @SedeId UNIQUEIDENTIFIER,
                    @UsuarioId UNIQUEIDENTIFIER,
                    @CantidadUnidadesMinimas INT,
                    @ReferenciaDocumento NVARCHAR(250),
                    @EstadoActivo INT,
                    @EstadoAgotado INT,
                    @TipoMovimientoVenta INT,
                    @FechaEmision DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    IF @CantidadUnidadesMinimas <= 0
                        THROW 50001, 'La cantidad a descontar debe ser mayor a cero.', 1;

                    DECLARE @FechaActual DATE = CAST(@FechaEmision AS DATE);
                    DECLARE @StockActual INT;
                    DECLARE @SedeRegistro UNIQUEIDENTIFIER;
                    DECLARE @EstadoActual INT;
                    DECLARE @LoteId UNIQUEIDENTIFIER;
                    DECLARE @ProductoId UNIQUEIDENTIFIER;
                    DECLARE @ProductoActivo BIT;
                    DECLARE @FechaVencimiento DATE;
                    DECLARE @NumeroLote NVARCHAR(100);
                    DECLARE @NombreProducto NVARCHAR(200);
                    DECLARE @PrecioCostoUnitario DECIMAL(18,2);

                    -- 1. Bloqueo a nivel de fila para evitar sobreventa concurrente
                    SELECT 
                        @StockActual = i.StockFisicoUnidades,
                        @SedeRegistro = i.SedeId,
                        @EstadoActual = i.Estado,
                        @LoteId = i.LoteId,
                        @ProductoId = l.ProductoId,
                        @ProductoActivo = p.Activo,
                        @FechaVencimiento = CAST(l.FechaVencimiento AS DATE),
                        @NumeroLote = l.CodigoLote,
                        @NombreProducto = p.NombreComercial,
                        @PrecioCostoUnitario = l.PrecioCostoUnitario
                    FROM InventarioSedes i WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN Lotes l ON i.LoteId = l.Id
                    INNER JOIN Productos p ON l.ProductoId = p.Id
                    WHERE i.Id = @InventarioSedeId;

                    IF @StockActual IS NULL
                        THROW 50002, 'El registro de inventario especificado no existe.', 1;

                    IF @SedeRegistro <> @SedeId
                        THROW 50003, 'El producto seleccionado no pertenece al inventario de su sede.', 1;

                    IF @ProductoActivo = 0
                    BEGIN
                        DECLARE @MsgProdInactivo NVARCHAR(250) = CONCAT('El producto ', @NombreProducto, ' se encuentra desactivado en el catálogo y no puede venderse.');
                        THROW 50008, @MsgProdInactivo, 1;
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

                    -- 2. Restricción FEFO estricta: impedir vender lote posterior si existe uno vigente más próximo a vencer con stock
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

                    -- 3. Validar stock disponible
                    IF @StockActual < @CantidadUnidadesMinimas
                    BEGIN
                        DECLARE @MsgStock NVARCHAR(300) = CONCAT('Stock insuficiente para ', @NombreProducto, ' (Lote ', @NumeroLote, '). Disponible: ', @StockActual, ', solicitado: ', @CantidadUnidadesMinimas, '.');
                        THROW 50006, @MsgStock, 1;
                    END

                    -- 4. Descontar stock y cambiar estado a Agotado si llega a 0
                    DECLARE @StockNuevo INT = @StockActual - @CantidadUnidadesMinimas;

                    UPDATE InventarioSedes
                    SET 
                        StockFisicoUnidades = @StockNuevo,
                        Estado = CASE WHEN @StockNuevo = 0 THEN @EstadoAgotado ELSE @EstadoActivo END
                    WHERE Id = @InventarioSedeId;

                    -- 5. Registrar SALIDA por venta en Kardex de la Sede
                    INSERT INTO TransaccionesKardex (
                        Id, SedeId, EsAlmacenCentral, LoteId, TipoMovimiento,
                        CantidadUnidades, StockAnteriorUnidades, StockNuevoUnidades,
                        PrecioCostoUnitario, Fecha, UsuarioId, ReferenciaDocumento
                    )
                    VALUES (
                        NEWID(), @SedeId, 0, @LoteId, @TipoMovimientoVenta,
                        @CantidadUnidadesMinimas, @StockActual, @StockNuevo,
                        @PrecioCostoUnitario, @FechaEmision, @UsuarioId, @ReferenciaDocumento
                    );
                END;";

            migrationBuilder.Sql(spPrepararVenta);
            migrationBuilder.Sql(spDescontarStock);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEA+ofEjONFmQuB6XC1MuJh/6ahsMMeTO2VJhlgKvYNyMDx/3SzMcJa0UN6iu3XCOVA==");
        }
    }
}
