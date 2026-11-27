using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    public partial class CorregirMermaYUltimaDanada_SPConfirmarRecepcion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var spConfirmacion = @"
                CREATE OR ALTER PROCEDURE sp_ConfirmarRecepcionSede
                    @InventarioSedeId UNIQUEIDENTIFIER,
                    @SedeUsuarioId UNIQUEIDENTIFIER,
                    @UsuarioRecibeId UNIQUEIDENTIFIER,
                    @CantidadDaniada INT,
                    @Observacion NVARCHAR(500) = NULL,
                    @EstadoEnTransito INT,
                    @EstadoActivo INT,
                    @EstadoAgotado INT,
                    @TipoMovimientoIngreso INT,
                    @TipoMovimientoMerma INT,
                    @FechaRecepcion DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    IF @CantidadDaniada < 0
                        THROW 51010, 'La cantidad dañada no puede ser un número negativo.', 1;

                    BEGIN TRANSACTION;

                    -- 1. Bloquear registro en tránsito
                    DECLARE @SedeRegistroId UNIQUEIDENTIFIER;
                    DECLARE @LoteId UNIQUEIDENTIFIER;
                    DECLARE @StockEnviado INT;
                    DECLARE @PrecioVentaUnidad DECIMAL(18,2);
                    DECLARE @EstadoActual INT;
                    DECLARE @PrecioCostoUnitario DECIMAL(18,2);

                    SELECT 
                        @SedeRegistroId = i.SedeId,
                        @LoteId = i.LoteId,
                        @StockEnviado = i.StockFisicoUnidades,
                        @PrecioVentaUnidad = i.PrecioVentaUnidad,
                        @EstadoActual = i.Estado,
                        @PrecioCostoUnitario = l.PrecioCostoUnitario
                    FROM InventarioSedes i WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN Lotes l ON i.LoteId = l.Id
                    WHERE i.Id = @InventarioSedeId;

                    IF @SedeRegistroId IS NULL
                        THROW 51011, 'El registro de inventario en tránsito no fue encontrado.', 1;

                    IF @SedeRegistroId <> @SedeUsuarioId
                        THROW 51012, 'No está autorizado para confirmar la recepción de un lote destinado a otra sede.', 1;

                    IF @EstadoActual <> @EstadoEnTransito
                        THROW 51013, 'Solo se pueden confirmar lotes que se encuentren en tránsito.', 1;

                    IF @CantidadDaniada > @StockEnviado
                        THROW 51014, 'La cantidad dañada no puede superar el stock enviado.', 1;

                    DECLARE @UnidadesBuenas INT = @StockEnviado - @CantidadDaniada;

                    -- 2. Buscar si ya existe otra capa activa con mismo Lote, Sede y PrecioVentaUnidad
                    DECLARE @CapaExistenteId UNIQUEIDENTIFIER;
                    DECLARE @StockAnteriorSede INT = 0;

                    SELECT TOP 1
                        @CapaExistenteId = Id,
                        @StockAnteriorSede = StockFisicoUnidades
                    FROM InventarioSedes WITH (UPDLOCK, ROWLOCK)
                    WHERE SedeId = @SedeRegistroId
                      AND LoteId = @LoteId
                      AND PrecioVentaUnidad = @PrecioVentaUnidad
                      AND Estado = @EstadoActivo
                      AND Id <> @InventarioSedeId;

                    DECLARE @StockNuevoSede INT = @StockAnteriorSede + @UnidadesBuenas;

                    -- 3. Fusionar en capa existente o activar el registro actual
                    IF @CapaExistenteId IS NOT NULL
                    BEGIN
                        UPDATE InventarioSedes
                        SET StockFisicoUnidades = @StockNuevoSede,
                            CantidadDaniadaAcumulada = CantidadDaniadaAcumulada + @CantidadDaniada,
                            UltimaCantidadDaniada = @CantidadDaniada,
                            UsuarioRecibeId = @UsuarioRecibeId,
                            FechaRecepcion = @FechaRecepcion,
                            ObservacionRecepcion = CASE WHEN NULLIF(LTRIM(RTRIM(@Observacion)), '') IS NOT NULL THEN @Observacion ELSE ObservacionRecepcion END
                        WHERE Id = @CapaExistenteId;

                        DELETE FROM InventarioSedes WHERE Id = @InventarioSedeId;
                    END
                    ELSE
                    BEGIN
                        DECLARE @NuevoEstado INT = CASE WHEN @UnidadesBuenas > 0 THEN @EstadoActivo ELSE @EstadoAgotado END;

                        UPDATE InventarioSedes
                        SET StockFisicoUnidades = @UnidadesBuenas,
                            CantidadDaniadaAcumulada = @CantidadDaniada,
                            UltimaCantidadDaniada = @CantidadDaniada,
                            UsuarioRecibeId = @UsuarioRecibeId,
                            FechaRecepcion = @FechaRecepcion,
                            ObservacionRecepcion = @Observacion,
                            Estado = @NuevoEstado
                        WHERE Id = @InventarioSedeId;
                    END

                    -- 4. Registrar INGRESO en Kardex de la Sede (unidades útiles recibidas)
                    IF @UnidadesBuenas > 0
                    BEGIN
                        DECLARE @RefIngreso NVARCHAR(250) = CASE 
                            WHEN @CantidadDaniada > 0 THEN CONCAT('Recepción en sede (Merma transporte: ', @CantidadDaniada, ' unid.)')
                            ELSE 'Recepción conforme en sede'
                        END;

                        INSERT INTO TransaccionesKardex (
                            Id, SedeId, EsAlmacenCentral, LoteId, TipoMovimiento,
                            CantidadUnidades, StockAnteriorUnidades, StockNuevoUnidades,
                            PrecioCostoUnitario, Fecha, UsuarioId, ReferenciaDocumento
                        )
                        VALUES (
                            NEWID(), @SedeRegistroId, 0, @LoteId, @TipoMovimientoIngreso,
                            @UnidadesBuenas, @StockAnteriorSede, @StockNuevoSede,
                            @PrecioCostoUnitario, @FechaRecepcion, @UsuarioRecibeId, @RefIngreso
                        );
                    END

                    -- 5. Registrar MERMA en Kardex de la Sede si hubo unidades dañadas en transporte
                    IF @CantidadDaniada > 0
                    BEGIN
                        DECLARE @RefMerma NVARCHAR(250) = CASE 
                            WHEN NULLIF(LTRIM(RTRIM(@Observacion)), '') IS NOT NULL 
                                THEN CONCAT('Merma en recepción: ', @Observacion)
                            ELSE 'Merma detectada en recepción en sede'
                        END;

                        INSERT INTO TransaccionesKardex (
                            Id, SedeId, EsAlmacenCentral, LoteId, TipoMovimiento,
                            CantidadUnidades, StockAnteriorUnidades, StockNuevoUnidades,
                            PrecioCostoUnitario, Fecha, UsuarioId, ReferenciaDocumento
                        )
                        VALUES (
                            NEWID(), @SedeRegistroId, 0, @LoteId, @TipoMovimientoMerma,
                            @CantidadDaniada, @StockNuevoSede, @StockNuevoSede,
                            @PrecioCostoUnitario, @FechaRecepcion, @UsuarioRecibeId, @RefMerma
                        );
                    END

                    COMMIT TRANSACTION;
                END;";

            migrationBuilder.Sql(spConfirmacion);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}