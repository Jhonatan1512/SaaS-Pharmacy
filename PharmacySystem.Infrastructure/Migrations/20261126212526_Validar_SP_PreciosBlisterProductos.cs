using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Validar_SP_PreciosBlisterProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELV4MfBOVuBhEhO34+W2K/sl5/Jbe8eo9dUD0SqhmGD1FhaYEfcSJ5JZY94qxro5wQ==");

            var spDespacho = @"
                CREATE OR ALTER PROCEDURE sp_DespacharLoteASede
                    @SedeId UNIQUEIDENTIFIER,
                    @LoteId UNIQUEIDENTIFIER,
                    @CantidadUnidades INT,
                    @PrecioVentaUnidad DECIMAL(18,2) = NULL,
                    @PrecioVentaBlister DECIMAL(18,2) = NULL,
                    @PrecioVentaCaja DECIMAL(18,2) = NULL,
                    @UsuarioId UNIQUEIDENTIFIER,
                    @EstadoEnTransito INT,
                    @TipoMovimientoSalida INT,
                    @FechaActual DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    IF @CantidadUnidades <= 0
                        THROW 51001, 'La cantidad a despachar debe ser mayor a cero.', 1;

                    BEGIN TRANSACTION;

                    -- 1. Validar Sede (existencia y estado activo)
                    DECLARE @NombreSede NVARCHAR(150);
                    DECLARE @SedeActiva BIT;

                    SELECT 
                        @NombreSede = Nombre,
                        @SedeActiva = Activa
                    FROM Sedes 
                    WHERE Id = @SedeId;

                    IF @NombreSede IS NULL
                        THROW 51002, 'La sede destino especificada no existe.', 1;

                    IF @SedeActiva = 0
                        THROW 51005, 'No se puede despachar mercadería a una sede que se encuentra inactiva.', 1;

                    -- 2. Bloquear fila de Almacén Central y obtener datos de Lote y Producto
                    DECLARE @InventarioCentralId UNIQUEIDENTIFIER;
                    DECLARE @StockAnteriorCentral INT;
                    DECLARE @CodigoLote NVARCHAR(100);
                    DECLARE @PrecioCostoUnitario DECIMAL(18,2);
                    DECLARE @PrecioProdUnidad DECIMAL(18,2);
                    DECLARE @PrecioProdBlister DECIMAL(18,2);
                    DECLARE @PrecioProdCaja DECIMAL(18,2);
                    DECLARE @UnidadesPorBlister INT;
                    DECLARE @UnidadesPorCaja INT;

                    SELECT 
                        @InventarioCentralId = iac.Id,
                        @StockAnteriorCentral = iac.StockFisicoUnidades,
                        @CodigoLote = l.CodigoLote,
                        @PrecioCostoUnitario = l.PrecioCostoUnitario,
                        @PrecioProdUnidad = p.PrecioUnidad,
                        @PrecioProdBlister = p.PrecioBlister,
                        @PrecioProdCaja = p.PrecioCaja,
                        @UnidadesPorBlister = p.UnidadesPorBlister,
                        @UnidadesPorCaja = p.UnidadesPorCaja
                    FROM InventariosAlmacenCentral iac WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN Lotes l ON iac.LoteId = l.Id
                    INNER JOIN Productos p ON l.ProductoId = p.Id
                    WHERE iac.LoteId = @LoteId;

                    IF @InventarioCentralId IS NULL
                        THROW 51003, 'El lote especificado no tiene registro en el Almacén Central.', 1;

                    IF @StockAnteriorCentral < @CantidadUnidades
                    BEGIN
                        DECLARE @MsgStock NVARCHAR(300) = CONCAT('Stock insuficiente en Almacén Central para el lote ', @CodigoLote, '. Disponible: ', @StockAnteriorCentral, ', solicitado: ', @CantidadUnidades, '.');
                        THROW 51004, @MsgStock, 1;
                    END

                    -- 2.1 Validar que no se registren precios para presentaciones inexistentes (ej. jarabes sin blíster)
                    DECLARE @TieneBlister BIT = CASE WHEN @UnidadesPorBlister > 1 OR ISNULL(@PrecioProdBlister, 0) > 0 THEN 1 ELSE 0 END;
                    DECLARE @TieneCaja BIT = CASE WHEN @UnidadesPorCaja > 1 OR ISNULL(@PrecioProdCaja, 0) > 0 THEN 1 ELSE 0 END;

                    IF ISNULL(@PrecioVentaBlister, 0) > 0 AND @TieneBlister = 0
                        THROW 51006, 'Este producto no cuenta con presentación en blíster; el precio de blíster debe ser 0.', 1;

                    IF ISNULL(@PrecioVentaCaja, 0) > 0 AND @TieneCaja = 0
                        THROW 51007, 'Este producto no cuenta con presentación en caja; el precio de caja debe ser 0.', 1;

                    -- 3. Descontar stock en Almacén Central
                    DECLARE @StockNuevoCentral INT = @StockAnteriorCentral - @CantidadUnidades;

                    UPDATE InventariosAlmacenCentral
                    SET StockFisicoUnidades = @StockNuevoCentral,
                        FechaUltimaActualizacion = @FechaActual
                    WHERE Id = @InventarioCentralId;

                    -- 4. Insertar registro EnTransito en InventarioSedes (hereda precios del catálogo si son NULL o 0)
                    DECLARE @PrecioFinalUnidad DECIMAL(18,2) = CASE WHEN ISNULL(@PrecioVentaUnidad, 0) > 0 THEN @PrecioVentaUnidad ELSE @PrecioProdUnidad END;
                    DECLARE @PrecioFinalBlister DECIMAL(18,2) = CASE WHEN ISNULL(@PrecioVentaBlister, 0) > 0 THEN @PrecioVentaBlister ELSE @PrecioProdBlister END;
                    DECLARE @PrecioFinalCaja DECIMAL(18,2) = CASE WHEN ISNULL(@PrecioVentaCaja, 0) > 0 THEN @PrecioVentaCaja ELSE @PrecioProdCaja END;

                    INSERT INTO InventarioSedes (
                        Id, SedeId, LoteId, StockFisicoUnidades,
                        PrecioVentaUnidad, PrecioVentaBlister, PrecioVentaCaja,
                        CantidadDaniadaAcumulada, UltimaCantidadDaniada, Estado
                    )
                    VALUES (
                        NEWID(), @SedeId, @LoteId, @CantidadUnidades,
                        @PrecioFinalUnidad, @PrecioFinalBlister, @PrecioFinalCaja,
                        0, 0, @EstadoEnTransito
                    );

                    -- 5. Registrar SALIDA en Kardex del Almacén Central
                    DECLARE @RefSalida NVARCHAR(250) = CONCAT('Envío a sede: ', @NombreSede);

                    INSERT INTO TransaccionesKardex (
                        Id, SedeId, EsAlmacenCentral, LoteId, TipoMovimiento,
                        CantidadUnidades, StockAnteriorUnidades, StockNuevoUnidades,
                        PrecioCostoUnitario, Fecha, UsuarioId, ReferenciaDocumento
                    )
                    VALUES (
                        NEWID(), NULL, 1, @LoteId, @TipoMovimientoSalida,
                        @CantidadUnidades, @StockAnteriorCentral, @StockNuevoCentral,
                        @PrecioCostoUnitario, @FechaActual, @UsuarioId, @RefSalida
                    );

                    COMMIT TRANSACTION;
                END;";

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

                    -- 1. Bloquear registro actual y obtener reglas de presentación del Producto
                    DECLARE @SedeId UNIQUEIDENTIFIER;
                    DECLARE @LoteId UNIQUEIDENTIFIER;
                    DECLARE @StockActual INT;
                    DECLARE @PrecioUnidadActual DECIMAL(18,2);
                    DECLARE @PrecioBlisterActual DECIMAL(18,2);
                    DECLARE @PrecioCajaActual DECIMAL(18,2);
                    DECLARE @UltimaDanada INT;
                    DECLARE @EstadoActual INT;
                    DECLARE @PrecioProdBlister DECIMAL(18,2);
                    DECLARE @PrecioProdCaja DECIMAL(18,2);
                    DECLARE @UnidadesPorBlister INT;
                    DECLARE @UnidadesPorCaja INT;

                    SELECT 
                        @SedeId = i.SedeId,
                        @LoteId = i.LoteId,
                        @StockActual = i.StockFisicoUnidades,
                        @PrecioUnidadActual = i.PrecioVentaUnidad,
                        @PrecioBlisterActual = i.PrecioVentaBlister,
                        @PrecioCajaActual = i.PrecioVentaCaja,
                        @UltimaDanada = i.UltimaCantidadDaniada,
                        @EstadoActual = i.Estado,
                        @PrecioProdBlister = p.PrecioBlister,
                        @PrecioProdCaja = p.PrecioCaja,
                        @UnidadesPorBlister = p.UnidadesPorBlister,
                        @UnidadesPorCaja = p.UnidadesPorCaja
                    FROM InventarioSedes i WITH (UPDLOCK, ROWLOCK)
                    INNER JOIN Lotes l ON i.LoteId = l.Id
                    INNER JOIN Productos p ON l.ProductoId = p.Id
                    WHERE i.Id = @InventarioSedeId;

                    IF @SedeId IS NULL
                        THROW 51020, 'El registro de inventario no existe.', 1;

                    -- Validar presentaciones permitidas para el producto
                    DECLARE @TieneBlister BIT = CASE WHEN @UnidadesPorBlister > 1 OR ISNULL(@PrecioProdBlister, 0) > 0 THEN 1 ELSE 0 END;
                    DECLARE @TieneCaja BIT = CASE WHEN @UnidadesPorCaja > 1 OR ISNULL(@PrecioProdCaja, 0) > 0 THEN 1 ELSE 0 END;

                    IF ISNULL(@PrecioVentaBlister, 0) > 0 AND @TieneBlister = 0
                        THROW 51021, 'Este producto no cuenta con presentación en blíster; el precio de blíster debe ser 0.', 1;

                    IF ISNULL(@PrecioVentaCaja, 0) > 0 AND @TieneCaja = 0
                        THROW 51022, 'Este producto no cuenta con presentación en caja; el precio de caja debe ser 0.', 1;

                    DECLARE @PrecioTargetUnidad DECIMAL(18,2) = CASE WHEN @PrecioVentaUnidad IS NOT NULL AND @PrecioVentaUnidad > 0 THEN @PrecioVentaUnidad ELSE @PrecioUnidadActual END;
                    DECLARE @PrecioTargetBlister DECIMAL(18,2) = CASE WHEN @PrecioVentaBlister IS NOT NULL AND @PrecioVentaBlister >= 0 THEN @PrecioVentaBlister ELSE @PrecioBlisterActual END;
                    DECLARE @PrecioTargetCaja DECIMAL(18,2) = CASE WHEN @PrecioVentaCaja IS NOT NULL AND @PrecioVentaCaja >= 0 THEN @PrecioVentaCaja ELSE @PrecioCajaActual END;

                    -- 2. Verificar si existe otra capa del mismo Lote, Sede, Estado y nuevo PrecioVentaUnidad
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
                    @FechaRecepcion DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    IF @CantidadDaniada < 0
                        THROW 51010, 'La cantidad dañada no puede ser un número negativo.', 1;

                    BEGIN TRANSACTION;

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

                    IF @CapaExistenteId IS NOT NULL
                    BEGIN
                        UPDATE InventarioSedes
                        SET StockFisicoUnidades = @StockNuevoSede,
                            CantidadDaniadaAcumulada = CantidadDaniadaAcumulada + @CantidadDaniada,
                            UltimaCantidadDaniada = CASE WHEN @CantidadDaniada > 0 THEN @CantidadDaniada ELSE UltimaCantidadDaniada END,
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

                    COMMIT TRANSACTION;
                END;";

            migrationBuilder.Sql(spDespacho);
            migrationBuilder.Sql(spActualizarPrecio);
            migrationBuilder.Sql(spConfirmacion);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEH4lPFOQo9tO2BAHNZJETkmysIYYQZXgTv3LiHdIB3IrFLjxt6g1Tp/MLBFD5+Oj6Q==");
        }
    }
}
