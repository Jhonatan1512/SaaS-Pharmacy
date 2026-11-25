using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionEntidadSolicitudReposicion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Observacion",
                table: "SolicitudesReposicion",
                newName: "RespuestaAdministrador");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLimiteRequerida",
                table: "SolicitudesReposicion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRespuesta",
                table: "SolicitudesReposicion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoSolicitud",
                table: "SolicitudesReposicion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioAprobadorId",
                table: "SolicitudesReposicion",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJOmPBAT+mQmE0U2Ezdqer6hTJjCg2NkV9BlXHDhWhBr9RwvZ2m+i2bzDNMA/EPa6Q==");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReposicion_UsuarioAprobadorId",
                table: "SolicitudesReposicion",
                column: "UsuarioAprobadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesReposicion_AspNetUsers_UsuarioAprobadorId",
                table: "SolicitudesReposicion",
                column: "UsuarioAprobadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesReposicion_AspNetUsers_UsuarioAprobadorId",
                table: "SolicitudesReposicion");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesReposicion_UsuarioAprobadorId",
                table: "SolicitudesReposicion");

            migrationBuilder.DropColumn(
                name: "FechaLimiteRequerida",
                table: "SolicitudesReposicion");

            migrationBuilder.DropColumn(
                name: "FechaRespuesta",
                table: "SolicitudesReposicion");

            migrationBuilder.DropColumn(
                name: "MotivoSolicitud",
                table: "SolicitudesReposicion");

            migrationBuilder.DropColumn(
                name: "UsuarioAprobadorId",
                table: "SolicitudesReposicion");

            migrationBuilder.RenameColumn(
                name: "RespuestaAdministrador",
                table: "SolicitudesReposicion",
                newName: "Observacion");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKm2nLZ9rFHCx2Y4Z5Y27YOviiQtHjRqtKOZB6zsiNOxTdFmcEN7K1q9jXsM9VwPCw==");
        }
    }
}
