using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qrcode_generator.Migrations
{
    /// <inheritdoc />
    public partial class CreateShipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShipmentId",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Carrier = table.Column<string>(type: "TEXT", nullable: false),
                    Consignee = table.Column<string>(type: "TEXT", nullable: false),
                    Departure = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShipmentId",
                table: "Products",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shipments_ShipmentId",
                table: "Products",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Shipments_ShipmentId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Products_ShipmentId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShipmentId",
                table: "Products");
        }
    }
}
