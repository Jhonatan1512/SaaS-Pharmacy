using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class configuracionIndexTablaLotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lotes_ProductoId",
                table: "Lotes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEL/UWjGxOcjVZYlNlLDKmyRSPSCbJQ0GHP0QGfpRYXHbx6TA/TjnNL5AORYmvsiQvw==");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_ProductoId_CodigoLote",
                table: "Lotes",
                columns: new[] { "ProductoId", "CodigoLote" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lotes_ProductoId_CodigoLote",
                table: "Lotes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDm0hwW93zyOXTQImVq1V64e0Ph1M/KZCXTWf0cFShRabOvkSm1Wn9wgeiAbVEGwCQ==");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_ProductoId",
                table: "Lotes",
                column: "ProductoId");
        }
    }
}
