using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionTablaInventariosSedeNuevoCampo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CantidadDaniada",
                table: "InventarioSedes",
                newName: "UltimaCantidadDaniada");

            migrationBuilder.AddColumn<int>(
                name: "CantidadDaniadaAcumulada",
                table: "InventarioSedes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMObrMRvmzqtjTyTYt4qvrVGh7DxUta+t6z5qJs7+lLZ+z40q25mUh8e9Zb+mO+77A==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadDaniadaAcumulada",
                table: "InventarioSedes");

            migrationBuilder.RenameColumn(
                name: "UltimaCantidadDaniada",
                table: "InventarioSedes",
                newName: "CantidadDaniada");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG5SBH9/aFqtpvdeFcA9OKXXHoIiHY+COzn/omMtLrVal37yP/QQpWRamb3s0Wn5hQ==");
        }
    }
}
