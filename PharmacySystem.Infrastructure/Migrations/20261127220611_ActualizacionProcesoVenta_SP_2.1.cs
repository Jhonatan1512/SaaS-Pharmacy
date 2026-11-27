using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionProcesoVenta_SP_21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG8g05hRWNgwAD7AS7wzFyGLq1F7Ok7YcOXjyBY1S979XjbbvPL0s1hRz325SR3tMg==");

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

            migrationBuilder.Sql(spPrepararVenta);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHqwUNhW9vXOAi+fb61xHhBPEyc6a9oVM6uXclgHWOIrARnsb5TUp3AW0Ug7+sbdKQ==");
        }
    }
}
