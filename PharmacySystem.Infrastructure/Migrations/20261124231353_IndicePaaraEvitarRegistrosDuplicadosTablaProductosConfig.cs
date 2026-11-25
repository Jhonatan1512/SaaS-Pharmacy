using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IndicePaaraEvitarRegistrosDuplicadosTablaProductosConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductosSedeConfig_SedeId",
                table: "ProductosSedeConfig");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPE6hnhWsOYqvNiehrBgsO4Fs51QOUEMV2q9xSb+PSVCcfu5zNIbcCRg09gQy8B5+Q==");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosSedeConfig_SedeId_ProductoId",
                table: "ProductosSedeConfig",
                columns: new[] { "SedeId", "ProductoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductosSedeConfig_SedeId_ProductoId",
                table: "ProductosSedeConfig");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDpxgGNS97fIvz/6EFDG8SiUe0baFdRjq7L573lxnp97q/UPRtLe3zMu8InvSbVbzA==");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosSedeConfig_SedeId",
                table: "ProductosSedeConfig",
                column: "SedeId");
        }
    }
}
