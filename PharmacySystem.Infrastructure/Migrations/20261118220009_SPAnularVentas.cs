using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SPAnularVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKm2nLZ9rFHCx2Y4Z5Y27YOviiQtHjRqtKOZB6zsiNOxTdFmcEN7K1q9jXsM9VwPCw==");

            var sql = @"
                CREATE OR ALTER PROCEDURE sp_AnularVenta
                    @VentaId UNIQUEIDENTIFIER,
                    @SedeId UNIQUEIDENTIFIER,
                    @MotivoAnulacion NVARCHAR(500),
                    @EstadoActivo INT,
                    @EstadoAgotado INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    IF @MotivoAnulacion IS NULL OR LTRIM(RTRIM(@MotivoAnulacion)) = ''
                    BEGIN
                        THROW 50011, 'El motivo de anulación es obligatorio.', 1;
                    END

                    DECLARE @SedeVenta UNIQUEIDENTIFIER;
                    DECLARE @YaAnulado BIT;
                    DECLARE @TurnoCajaId UNIQUEIDENTIFIER;
                    DECLARE @TurnoAbierto BIT;

                    BEGIN TRANSACTION;

                    -- Bloquear la fila de la venta para evitar doble anulación concurrente
                    SELECT 
                        @SedeVenta = v.SedeId,
                        @YaAnulado = v.Anulado,
                        @TurnoCajaId = v.TurnoCajaId
                    FROM Ventas v WITH (UPDLOCK, ROWLOCK)
                    WHERE v.Id = @VentaId;

                    IF @SedeVenta IS NULL
                    BEGIN
                        THROW 50012, 'La venta especificada no existe.', 1;
                    END

                    IF @SedeVenta <> @SedeId
                    BEGIN
                        THROW 50013, 'No tiene permisos para anular ventas de otra sede.', 1;
                    END

                    IF @YaAnulado = 1
                    BEGIN
                        THROW 50014, 'Esta venta ya se encuentra anulada.', 1;
                    END

                    -- Validar que el turno de caja asociado aún esté abierto
                    IF @TurnoCajaId IS NOT NULL
                    BEGIN
                        SELECT @TurnoAbierto = EstadoAbierto
                        FROM TurnosCaja WITH (UPDLOCK)
                        WHERE Id = @TurnoCajaId;

                        IF @TurnoAbierto = 0
                        BEGIN
                            THROW 50015, 'No se puede anular una venta de un turno de caja que ya fue cerrado.', 1;
                        END
                    END

                    -- Devolver el stock a la sede (agrupando por lote en caso de líneas repetidas)
                    --    Si el lote había quedado Agotado, vuelve automáticamente a Activo
                    UPDATE i
                    SET 
                        i.StockFisicoUnidades = i.StockFisicoUnidades + agg.TotalUnidadesDevolver,
                        i.Estado = CASE 
                                     WHEN i.Estado = @EstadoAgotado THEN @EstadoActivo 
                                     ELSE i.Estado 
                                   END
                    FROM InventarioSedes i WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN (
                        SELECT InventarioSedeId, SUM(CantidadUnidadesMinimas) AS TotalUnidadesDevolver
                        FROM DetallesVenta
                        WHERE VentaId = @VentaId
                        GROUP BY InventarioSedeId
                    ) agg ON i.Id = agg.InventarioSedeId;

                    -- Marcar la venta como anulada conservando DetallesVenta y PagosVenta
                    UPDATE Ventas
                    SET 
                        Anulado = 1,
                        MotivoAnulacion = LTRIM(RTRIM(@MotivoAnulacion))
                    WHERE Id = @VentaId;

                    COMMIT TRANSACTION;
                END;
                ";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKDz4ow/3t0q3oLsKrZXSTriu2jG3A3vECV76XRJ/tF62gmXyZUUnbDr5qrMUaNWDQ==");
        }
    }
}
