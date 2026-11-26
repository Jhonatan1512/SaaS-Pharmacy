using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SP_ActualizarPrecioInventarioSedeYsp_CorregirMermaRecepcionSede : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGwNWTbZod/rf6zRRKRTht9vUZsjBWKem5VcUErQ3c94xMspFIUTxgK5V9jlIULFIg==");

            var spActualizarPrecio = @"
                CREATE OR ALTER PROCEDURE sp_ActualizarPrecioInventarioSede
                    @InventarioSedeId UNIQUEIDENTIFIER,
                    @PrecioVentaUnidad DECIMAL(18,2) = NULL,
                    @PrecioVentaBlister DECIMAL(18,2) = NULL,
                    @PrecioVentaCaja DECIMAL(18,2) = NULL,
                    @EstadoAgotado INT,
                    @EstadoActivo INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    BEGIN TRANSACTION;

                    DECLARE @SedeId UNIQUEIDENTIFIER;
                    DECLARE @LoteId UNIQUEIDENTIFIER;
                    DECLARE @StockActual INT;
                    DECLARE @PrecioUnidadActual DECIMAL(18,2);
                    DECLARE @PrecioBlisterActual DECIMAL(18,2);
                    DECLARE @PrecioCajaActual DECIMAL(18,2);
                    DECLARE @UltimaDanada INT;
                    DECLARE @EstadoActual INT;

                    SELECT 
                        @SedeId = SedeId,
                        @LoteId = LoteId,
                        @StockActual = StockFisicoUnidades,
                        @PrecioUnidadActual = PrecioVentaUnidad,
                        @PrecioBlisterActual = PrecioVentaBlister,
                        @PrecioCajaActual = PrecioVentaCaja,
                        @UltimaDanada = UltimaCantidadDaniada,
                        @EstadoActual = Estado
                    FROM InventarioSedes WITH (UPDLOCK, ROWLOCK)
                    WHERE Id = @InventarioSedeId;

                    IF @SedeId IS NULL
                        THROW 51020, 'El registro de inventario no existe.', 1;

                    DECLARE @PrecioTargetUnidad DECIMAL(18,2) = CASE WHEN @PrecioVentaUnidad IS NOT NULL AND @PrecioVentaUnidad > 0 THEN @PrecioVentaUnidad ELSE @PrecioUnidadActual END;
                    DECLARE @PrecioTargetBlister DECIMAL(18,2) = CASE WHEN @PrecioVentaBlister IS NOT NULL AND @PrecioVentaBlister >= 0 THEN @PrecioVentaBlister ELSE @PrecioBlisterActual END;
                    DECLARE @PrecioTargetCaja DECIMAL(18,2) = CASE WHEN @PrecioVentaCaja IS NOT NULL AND @PrecioVentaCaja >= 0 THEN @PrecioVentaCaja ELSE @PrecioCajaActual END;

                    DECLARE @CapaDestinoId UNIQUEIDENTIFIER;

                    SELECT TOP 1 @CapaDestinoId = Id
                    FROM InventarioSedes WITH (UPDLOCK, ROWLOCK)
                    WHERE SedeId = @SedeId
                      AND LoteId = @LoteId
                      AND PrecioVentaUnidad = @PrecioTargetUnidad
                      AND Estado = @EstadoActual
                      AND Id <> @InventarioSedeId;

                    IF @CapaDestinoId IS NOT NULL
                    BEGIN
                        UPDATE InventarioSedes
                        SET StockFisicoUnidades = StockFisicoUnidades + @StockActual,
                            CantidadDaniadaAcumulada = CantidadDaniadaAcumulada + @UltimaDanada,
                            UltimaCantidadDaniada = CASE WHEN @UltimaDanada > 0 THEN @UltimaDanada ELSE UltimaCantidadDaniada END,
                            PrecioVentaUnidad = @PrecioTargetUnidad,
                            PrecioVentaBlister = @PrecioTargetBlister,
                            PrecioVentaCaja = @PrecioTargetCaja,
                            Estado = CASE WHEN @EstadoActual = @EstadoAgotado AND (StockFisicoUnidades + @StockActual) > 0 THEN @EstadoActivo ELSE Estado END
                        WHERE Id = @CapaDestinoId;

                        DELETE FROM InventarioSedes WHERE Id = @InventarioSedeId;
                    END
                    ELSE
                    BEGIN
                        UPDATE InventarioSedes
                        SET PrecioVentaUnidad = @PrecioTargetUnidad,
                            PrecioVentaBlister = @PrecioTargetBlister,
                            PrecioVentaCaja = @PrecioTargetCaja
                        WHERE Id = @InventarioSedeId;
                    END

                    COMMIT TRANSACTION;
                END;";

            var spCorregirMerma = @"
                CREATE OR ALTER PROCEDURE sp_CorregirMermaRecepcionSede
                    @InventarioSedeId UNIQUEIDENTIFIER,
                    @SedeUsuarioId UNIQUEIDENTIFIER,
                    @UsuarioId UNIQUEIDENTIFIER,
                    @NuevaCantidadDanada INT,
                    @MotivoCorreccion NVARCHAR(500) = NULL,
                    @EstadoActivo INT,
                    @EstadoAgotado INT,
                    @TipoMovimientoAjuste INT,
                    @FechaActual DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    IF @NuevaCantidadDanada < 0
                        THROW 51030, 'La cantidad dañada no puede ser un número negativo.', 1;

                    BEGIN TRANSACTION;

                    DECLARE @SedeRegistroId UNIQUEIDENTIFIER;
                    DECLARE @LoteId UNIQUEIDENTIFIER;
                    DECLARE @StockAnterior INT;
                    DECLARE @UltimaDanada INT;
                    DECLARE @DanadaAcumulada INT;
                    DECLARE @EstadoActual INT;
                    DECLARE @PrecioCostoUnitario DECIMAL(18,2);

                    SELECT 
                        @SedeRegistroId = i.SedeId,
                        @LoteId = i.LoteId,
                        @StockAnterior = i.StockFisicoUnidades,
                        @UltimaDanada = i.UltimaCantidadDaniada,
                        @DanadaAcumulada = i.CantidadDaniadaAcumulada,
                        @EstadoActual = i.Estado,
                        @PrecioCostoUnitario = l.PrecioCostoUnitario
                    FROM InventarioSedes i WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN Lotes l ON i.LoteId = l.Id
                    WHERE i.Id = @InventarioSedeId;

                    IF @SedeRegistroId IS NULL
                        THROW 51031, 'El registro de inventario no fue encontrado.', 1;

                    IF @SedeRegistroId <> @SedeUsuarioId
                        THROW 51032, 'No tiene permisos para modificar inventarios de otra sede.', 1;

                    IF @EstadoActual <> @EstadoActivo AND @EstadoActual <> @EstadoAgotado
                        THROW 51033, 'Solo se pueden corregir mermas de registros recepcionados.', 1;

                    DECLARE @DiferenciaStock INT = @UltimaDanada - @NuevaCantidadDanada;
                    DECLARE @StockNuevo INT = @StockAnterior + @DiferenciaStock;
                    DECLARE @NuevaDanadaAcumulada INT = @DanadaAcumulada - @DiferenciaStock;

                    IF @StockNuevo < 0
                    BEGIN
                        DECLARE @MsgStock NVARCHAR(300) = CONCAT('No se puede ajustar la merma. El stock vendible actual (', @StockAnterior, ') es menor a la reducción solicitada.');
                        THROW 51034, @MsgStock, 1;
                    END

                    IF @NuevaDanadaAcumulada < 0
                        THROW 51035, 'El ajuste dejaría el acumulado total de mermas en un número negativo.', 1;

                    DECLARE @NuevoEstado INT = CASE WHEN @StockNuevo > 0 THEN @EstadoActivo ELSE @EstadoAgotado END;
                    DECLARE @NuevaObs NVARCHAR(500) = CASE 
                        WHEN NULLIF(LTRIM(RTRIM(@MotivoCorreccion)), '') IS NOT NULL THEN CONCAT('[CORREGIDO]: ', @MotivoCorreccion)
                        ELSE NULL
                    END;

                    UPDATE InventarioSedes
                    SET StockFisicoUnidades = @StockNuevo,
                        CantidadDaniadaAcumulada = @NuevaDanadaAcumulada,
                        UltimaCantidadDaniada = @NuevaCantidadDanada,
                        ObservacionRecepcion = ISNULL(@NuevaObs, ObservacionRecepcion),
                        Estado = @NuevoEstado
                    WHERE Id = @InventarioSedeId;

                    IF @DiferenciaStock <> 0
                    BEGIN
                        DECLARE @RefAjuste NVARCHAR(250) = CONCAT('Corrección merma recepción: ', ISNULL(@MotivoCorreccion, 'Sin detalle'));

                        INSERT INTO TransaccionesKardex (
                            Id, SedeId, EsAlmacenCentral, LoteId, TipoMovimiento,
                            CantidadUnidades, StockAnteriorUnidades, StockNuevoUnidades,
                            PrecioCostoUnitario, Fecha, UsuarioId, ReferenciaDocumento
                        )
                        VALUES (
                            NEWID(), @SedeRegistroId, 0, @LoteId, @TipoMovimientoAjuste,
                            ABS(@DiferenciaStock), @StockAnterior, @StockNuevo,
                            @PrecioCostoUnitario, @FechaActual, @UsuarioId, @RefAjuste
                        );
                    END

                    COMMIT TRANSACTION;
                END;";

            migrationBuilder.Sql(spActualizarPrecio);
            migrationBuilder.Sql(spCorregirMerma);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ActualizarPrecioInventarioSede;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CorregirMermaRecepcionSede;");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFOw1Za5lLxk3uSl5qOH6occh7jY/424UYNrHjrqUovhseCcBoFqlD9D01WaccoBEA==");
        }
    }
}