using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qrcode_generator.Migrations
{
    /// <inheritdoc />
    public partial class CreateShipmentProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Shipments_ContractId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Shipments_ShipmentId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ShipmentId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShipmentId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShipmentProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShipmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    Loading = table.Column<int>(type: "INTEGER", nullable: true),
                    Unloading = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShipmentProducts_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentProducts_ProductId",
                table: "ShipmentProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentProducts_ShipmentId",
                table: "ShipmentProducts",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Contracts_ContractId",
                table: "Products",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Contracts_ContractId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ShipmentProducts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Shipments");

            migrationBuilder.AddColumn<int>(
                name: "ShipmentId",
                table: "Products",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShipmentId",
                table: "Products",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shipments_ContractId",
                table: "Products",
                column: "ContractId",
                principalTable: "Shipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shipments_ShipmentId",
                table: "Products",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id");
        }
    }
}
