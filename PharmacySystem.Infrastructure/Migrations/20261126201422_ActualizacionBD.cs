using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFOw1Za5lLxk3uSl5qOH6occh7jY/424UYNrHjrqUovhseCcBoFqlD9D01WaccoBEA==");

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

                    -- 1. Validar Sede
                    DECLARE @NombreSede NVARCHAR(150);
                    SELECT @NombreSede = Nombre FROM Sedes WHERE Id = @SedeId;
                    IF @NombreSede IS NULL
                        THROW 51002, 'La sede destino especificada no existe.', 1;

                    -- 2. Bloquear fila de Almacén Central y obtener datos de Lote y Producto
                    DECLARE @InventarioCentralId UNIQUEIDENTIFIER;
                    DECLARE @StockAnteriorCentral INT;
                    DECLARE @CodigoLote NVARCHAR(100);
                    DECLARE @PrecioCostoUnitario DECIMAL(18,2);
                    DECLARE @PrecioProdUnidad DECIMAL(18,2);
                    DECLARE @PrecioProdBlister DECIMAL(18,2);
                    DECLARE @PrecioProdCaja DECIMAL(18,2);

                    SELECT 
                        @InventarioCentralId = iac.Id,
                        @StockAnteriorCentral = iac.StockFisicoUnidades,
                        @CodigoLote = l.CodigoLote,
                        @PrecioCostoUnitario = l.PrecioCostoUnitario,
                        @PrecioProdUnidad = p.PrecioUnidad,
                        @PrecioProdBlister = p.PrecioBlister,
                        @PrecioProdCaja = p.PrecioCaja
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

                    -- 3. Descontar stock en Almacén Central
                    DECLARE @StockNuevoCentral INT = @StockAnteriorCentral - @CantidadUnidades;

                    UPDATE InventariosAlmacenCentral
                    SET StockFisicoUnidades = @StockNuevoCentral,
                        FechaUltimaActualizacion = @FechaActual
                    WHERE Id = @InventarioCentralId;

                    -- 4. Insertar registro EnTransito en InventarioSedes (hereda precios del catálogo si son NULL o <= 0)
                    DECLARE @PrecioFinalUnidad DECIMAL(18,2) = CASE WHEN ISNULL(@PrecioVentaUnidad, 0) > 0 THEN @PrecioVentaUnidad ELSE @PrecioProdUnidad END;
                    DECLARE @PrecioFinalBlister DECIMAL(18,2) = CASE WHEN @PrecioVentaBlister IS NOT NULL AND @PrecioVentaBlister >= 0 THEN @PrecioVentaBlister ELSE @PrecioProdBlister END;
                    DECLARE @PrecioFinalCaja DECIMAL(18,2) = CASE WHEN @PrecioVentaCaja IS NOT NULL AND @PrecioVentaCaja >= 0 THEN @PrecioVentaCaja ELSE @PrecioProdCaja END;

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

                    -- 4. Registrar INGRESO en Kardex de la Sede (si ingresaron unidades útiles)
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
            migrationBuilder.Sql(spConfirmacion);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DespacharLoteASede;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ConfirmarRecepcionSede;");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFUlyzLnFuRwZ9tV/ZurZB22W6LQzHRao5HbBr0eDWwZT/MCvanR8HrMXMl8ZPmiPg==");
        }
    }
}