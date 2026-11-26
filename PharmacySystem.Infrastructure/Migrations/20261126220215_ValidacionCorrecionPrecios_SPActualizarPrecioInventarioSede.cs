using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ValidacionCorrecionPrecios_SPActualizarPrecioInventarioSede : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEA+ofEjONFmQuB6XC1MuJh/6ahsMMeTO2VJhlgKvYNyMDx/3SzMcJa0UN6iu3XCOVA==");

            var spActualizarPrecio = @"
                CREATE OR ALTER PROCEDURE sp_ActualizarPrecioInventarioSede
                    @InventarioSedeId UNIQUEIDENTIFIER,
                    @PrecioVentaUnidad DECIMAL(18,2) = NULL,
                    @PrecioVentaBlister DECIMAL(18,2) = NULL,
                    @PrecioVentaCaja DECIMAL(18,2) = NULL,
                    @EstadoEnTransito INT,
                    @EstadoAgotado INT,
                    @EstadoActivo INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    BEGIN TRANSACTION;

                    -- 1. Bloquear registro actual y obtener reglas de presentación del Producto
                    DECLARE @SedeId UNIQUEIDENTIFIER;
                    DECLARE @PrecioUnidadActual DECIMAL(18,2);
                    DECLARE @PrecioBlisterActual DECIMAL(18,2);
                    DECLARE @PrecioCajaActual DECIMAL(18,2);
                    DECLARE @EstadoActual INT;
                    DECLARE @PrecioProdBlister DECIMAL(18,2);
                    DECLARE @PrecioProdCaja DECIMAL(18,2);
                    DECLARE @UnidadesPorBlister INT;
                    DECLARE @UnidadesPorCaja INT;

                    SELECT 
                        @SedeId = i.SedeId,
                        @PrecioUnidadActual = i.PrecioVentaUnidad,
                        @PrecioBlisterActual = i.PrecioVentaBlister,
                        @PrecioCajaActual = i.PrecioVentaCaja,
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

                    IF @EstadoActual <> @EstadoEnTransito
                        THROW 51023, 'Solo se pueden corregir los precios de lotes que aún se encuentran en tránsito.', 1;

                    -- 2. Validar presentaciones permitidas para el producto
                    DECLARE @TieneBlister BIT = CASE WHEN @UnidadesPorBlister > 1 OR ISNULL(@PrecioProdBlister, 0) > 0 THEN 1 ELSE 0 END;
                    DECLARE @TieneCaja BIT = CASE WHEN @UnidadesPorCaja > 1 OR ISNULL(@PrecioProdCaja, 0) > 0 THEN 1 ELSE 0 END;

                    IF ISNULL(@PrecioVentaBlister, 0) > 0 AND @TieneBlister = 0
                        THROW 51021, 'Este producto no cuenta con presentación en blíster; el precio de blíster debe ser 0.', 1;

                    IF ISNULL(@PrecioVentaCaja, 0) > 0 AND @TieneCaja = 0
                        THROW 51022, 'Este producto no cuenta con presentación en caja; el precio de caja debe ser 0.', 1;

                    -- 3. Calcular precios finales y actualizar
                    DECLARE @PrecioTargetUnidad DECIMAL(18,2) = CASE WHEN @PrecioVentaUnidad IS NOT NULL AND @PrecioVentaUnidad > 0 THEN @PrecioVentaUnidad ELSE @PrecioUnidadActual END;
                    DECLARE @PrecioTargetBlister DECIMAL(18,2) = CASE WHEN @PrecioVentaBlister IS NOT NULL AND @PrecioVentaBlister >= 0 THEN @PrecioVentaBlister ELSE @PrecioBlisterActual END;
                    DECLARE @PrecioTargetCaja DECIMAL(18,2) = CASE WHEN @PrecioVentaCaja IS NOT NULL AND @PrecioVentaCaja >= 0 THEN @PrecioVentaCaja ELSE @PrecioCajaActual END;

                    UPDATE InventarioSedes
                    SET PrecioVentaUnidad = @PrecioTargetUnidad,
                        PrecioVentaBlister = @PrecioTargetBlister,
                        PrecioVentaCaja = @PrecioTargetCaja
                    WHERE Id = @InventarioSedeId;

                    COMMIT TRANSACTION;
                END;";

            migrationBuilder.Sql(spActualizarPrecio);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELV4MfBOVuBhEhO34+W2K/sl5/Jbe8eo9dUD0SqhmGD1FhaYEfcSJ5JZY94qxro5wQ==");
        }
    }
}