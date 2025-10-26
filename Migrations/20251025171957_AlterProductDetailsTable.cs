using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qrcode_generator.Migrations
{
    /// <inheritdoc />
    public partial class AlterProductDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Unloading",
                table: "ShipmentProducts",
                newName: "Unloaded");

            migrationBuilder.RenameColumn(
                name: "Loading",
                table: "ShipmentProducts",
                newName: "Loaded");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ProductDetails",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "ProductDetails",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "SequenceNo",
                table: "ProductDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShortId",
                table: "ProductDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SequenceNo",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "ShortId",
                table: "ProductDetails");

            migrationBuilder.RenameColumn(
                name: "Unloaded",
                table: "ShipmentProducts",
                newName: "Unloading");

            migrationBuilder.RenameColumn(
                name: "Loaded",
                table: "ShipmentProducts",
                newName: "Loading");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ProductDetails",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ProductDetails",
                newName: "Key");
        }
    }
}
