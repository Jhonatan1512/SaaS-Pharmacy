using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    public partial class ActualizacionAnulacionVenta_SP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
                CREATE OR ALTER PROCEDURE sp_AnularVenta
                    @VentaId UNIQUEIDENTIFIER,
                    @SedeId UNIQUEIDENTIFIER,
                    @UsuarioId UNIQUEIDENTIFIER,
                    @MotivoAnulacion NVARCHAR(500),
                    @EstadoActivo INT,
                    @EstadoAgotado INT,
                    @TipoMovimientoAjuste INT,
                    @FechaAnulacion DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    IF @MotivoAnulacion IS NULL OR LTRIM(RTRIM(@MotivoAnulacion)) = ''
                        THROW 50011, 'El motivo de anulación es obligatorio.', 1;

                    DECLARE @SedeVenta UNIQUEIDENTIFIER;
                    DECLARE @YaAnulado BIT;
                    DECLARE @TurnoCajaId UNIQUEIDENTIFIER;
                    DECLARE @Serie NVARCHAR(20);
                    DECLARE @Numero NVARCHAR(20);
                    DECLARE @EstadoSunat NVARCHAR(50);
                    DECLARE @FechaActual DATE = CAST(@FechaAnulacion AS DATE);

                    BEGIN TRANSACTION;

                    -- 1. Validar que la sede esté activa
                    DECLARE @SedeActiva BIT;
                    SELECT @SedeActiva = Activa FROM Sedes WHERE Id = @SedeId;

                    IF @SedeActiva IS NULL OR @SedeActiva = 0
                        THROW 50016, 'No se pueden anular ventas porque la sede no existe o se encuentra inactiva.', 1;

                    -- 2. Bloquear la fila de la venta para evitar doble anulación concurrente
                    SELECT 
                        @SedeVenta = v.SedeId,
                        @YaAnulado = v.Anulado,
                        @TurnoCajaId = v.TurnoCajaId,
                        @Serie = v.Serie,
                        @Numero = v.Numero,
                        @EstadoSunat = v.EstadoSunat
                    FROM Ventas v WITH (UPDLOCK, ROWLOCK)
                    WHERE v.Id = @VentaId;

                    IF @SedeVenta IS NULL
                        THROW 50012, 'La venta especificada no existe.', 1;

                    IF @SedeVenta <> @SedeId
                        THROW 50013, 'No tiene permisos para anular ventas de otra sede.', 1;

                    IF @YaAnulado = 1
                        THROW 50014, 'Esta venta ya se encuentra anulada.', 1;

                    IF @EstadoSunat = 'ACEPTADO'
                        THROW 50017, 'Este comprobante ya fue aceptado por SUNAT. Debe emitir una Nota de Crédito en lugar de anularlo directamente.', 1;

                    -- 3. Validar que el turno de caja asociado exista y aún esté abierto
                    IF @TurnoCajaId IS NOT NULL
                    BEGIN
                        DECLARE @TurnoAbierto BIT;
                        SELECT @TurnoAbierto = EstadoAbierto
                        FROM TurnosCaja WITH (UPDLOCK)
                        WHERE Id = @TurnoCajaId;

                        IF @TurnoAbierto IS NULL OR @TurnoAbierto = 0
                            THROW 50015, 'No se puede anular una venta de un turno de caja que ya fue cerrado.', 1;
                    END

                    -- 4. Capturar detalles agrupados por InventarioSedeId bloqueando las filas de inventario
                    DECLARE @ItemsDevolver TABLE (
                        InventarioSedeId UNIQUEIDENTIFIER PRIMARY KEY,
                        LoteId UNIQUEIDENTIFIER,
                        CodigoLote NVARCHAR(100),
                        NombreProducto NVARCHAR(200),
                        FechaVencimiento DATE,
                        PrecioCostoUnitario DECIMAL(18,2),
                        EstadoActual INT,
                        StockAnterior INT,
                        UnidadesDevolver INT
                    );

                    INSERT INTO @ItemsDevolver (
                        InventarioSedeId, LoteId, CodigoLote, NombreProducto,
                        FechaVencimiento, PrecioCostoUnitario, EstadoActual,
                        StockAnterior, UnidadesDevolver
                    )
                    SELECT 
                        i.Id,
                        i.LoteId,
                        l.CodigoLote,
                        p.NombreComercial,
                        CAST(l.FechaVencimiento AS DATE),
                        l.PrecioCostoUnitario,
                        i.Estado,
                        i.StockFisicoUnidades,
                        agg.TotalUnidadesDevolver
                    FROM InventarioSedes i WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN Lotes l ON i.LoteId = l.Id
                    INNER JOIN Productos p ON l.ProductoId = p.Id
                    INNER JOIN (
                        SELECT InventarioSedeId, SUM(CantidadUnidadesMinimas) AS TotalUnidadesDevolver
                        FROM DetallesVenta
                        WHERE VentaId = @VentaId
                        GROUP BY InventarioSedeId
                    ) agg ON i.Id = agg.InventarioSedeId;

                    IF NOT EXISTS (SELECT 1 FROM @ItemsDevolver)
                        THROW 50018, 'La venta no contiene detalles de inventario válidos para revertir.', 1;

                    -- 5. Validar que ningún lote a reingresar esté vencido (seguridad sanitaria)
                    DECLARE @LoteVencido NVARCHAR(100);
                    DECLARE @ProdVencido NVARCHAR(200);

                    SELECT TOP 1 
                        @LoteVencido = CodigoLote,
                        @ProdVencido = NombreProducto
                    FROM @ItemsDevolver
                    WHERE FechaVencimiento < @FechaActual;

                    IF @LoteVencido IS NOT NULL
                    BEGIN
                        DECLARE @MsgVenc NVARCHAR(350) = CONCAT(
                            'No se puede anular la venta porque el lote ', @LoteVencido,
                            ' de ', @ProdVencido, ' ya se encuentra vencido y no puede reingresar al stock vendible.'
                        );
                        THROW 50019, @MsgVenc, 1;
                    END

                    -- 6. Devolver el stock a la sede y reactivar solo si estaba Agotado y vigente
                    UPDATE i
                    SET 
                        i.StockFisicoUnidades = d.StockAnterior + d.UnidadesDevolver,
                        i.Estado = CASE 
                                     WHEN d.EstadoActual = @EstadoAgotado THEN @EstadoActivo 
                                     ELSE d.EstadoActual 
                                   END
                    FROM InventarioSedes i
                    INNER JOIN @ItemsDevolver d ON i.Id = d.InventarioSedeId;

                    -- 7. Registrar REINGRESO por anulación en TransaccionesKardex de la sede
                    DECLARE @RefAnulacion NVARCHAR(250) = CONCAT('Anulación Venta ', @Serie, '-', @Numero, ': ', LTRIM(RTRIM(@MotivoAnulacion)));

                    INSERT INTO TransaccionesKardex (
                        Id, SedeId, EsAlmacenCentral, LoteId, TipoMovimiento,
                        CantidadUnidades, StockAnteriorUnidades, StockNuevoUnidades,
                        PrecioCostoUnitario, Fecha, UsuarioId, ReferenciaDocumento
                    )
                    SELECT 
                        NEWID(),
                        @SedeId,
                        0,
                        d.LoteId,
                        @TipoMovimientoAjuste,
                        d.UnidadesDevolver,
                        d.StockAnterior,
                        d.StockAnterior + d.UnidadesDevolver,
                        d.PrecioCostoUnitario,
                        @FechaAnulacion,
                        @UsuarioId,
                        LEFT(@RefAnulacion, 250)
                    FROM @ItemsDevolver d;

                    -- 8. Marcar la venta como anulada
                    UPDATE Ventas
                    SET 
                        Anulado = 1,
                        MotivoAnulacion = LTRIM(RTRIM(@MotivoAnulacion)),
                        EstadoSunat = CASE WHEN EstadoSunat = 'NO_EMITIDO' THEN 'ANULADO' ELSE EstadoSunat END
                    WHERE Id = @VentaId;

                    COMMIT TRANSACTION;
                END;";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}