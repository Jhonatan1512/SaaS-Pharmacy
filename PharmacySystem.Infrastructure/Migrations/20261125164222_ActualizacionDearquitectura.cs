using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionDearquitectura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SedeId",
                table: "TransaccionesKardex",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "EsAlmacenCentral",
                table: "TransaccionesKardex",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCostoUnitario",
                table: "TransaccionesKardex",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CantidadInicialUnidades",
                table: "Lotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InventariosAlmacenCentral",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockFisicoUnidades = table.Column<int>(type: "int", nullable: false),
                    StockMinimoUnidades = table.Column<int>(type: "int", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventariosAlmacenCentral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventariosAlmacenCentral_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFUlyzLnFuRwZ9tV/ZurZB22W6LQzHRao5HbBr0eDWwZT/MCvanR8HrMXMl8ZPmiPg==");

            migrationBuilder.CreateIndex(
                name: "IX_InventariosAlmacenCentral_LoteId",
                table: "InventariosAlmacenCentral",
                column: "LoteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventariosAlmacenCentral");

            migrationBuilder.DropColumn(
                name: "EsAlmacenCentral",
                table: "TransaccionesKardex");

            migrationBuilder.DropColumn(
                name: "PrecioCostoUnitario",
                table: "TransaccionesKardex");

            migrationBuilder.DropColumn(
                name: "CantidadInicialUnidades",
                table: "Lotes");

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeId",
                table: "TransaccionesKardex",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPE6hnhWsOYqvNiehrBgsO4Fs51QOUEMV2q9xSb+PSVCcfu5zNIbcCRg09gQy8B5+Q==");
        }
    }
}
