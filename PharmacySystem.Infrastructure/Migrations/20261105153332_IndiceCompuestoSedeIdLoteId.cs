using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IndiceCompuestoSedeIdLoteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventarioSedes_SedeId",
                table: "InventarioSedes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDL7pO8EpA7aHB/E1h6x8iy+wXU1biwgOosnc+F2NQ3xOK0iypU/5FlfCDJUkBy9Ag==");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioSedes_SedeId_LoteId",
                table: "InventarioSedes",
                columns: new[] { "SedeId", "LoteId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventarioSedes_SedeId_LoteId",
                table: "InventarioSedes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEL/UWjGxOcjVZYlNlLDKmyRSPSCbJQ0GHP0QGfpRYXHbx6TA/TjnNL5AORYmvsiQvw==");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioSedes_SedeId",
                table: "InventarioSedes",
                column: "SedeId");
        }
    }
}
