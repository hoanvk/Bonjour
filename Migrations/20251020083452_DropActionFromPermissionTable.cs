using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qrcode_generator.Migrations
{
    /// <inheritdoc />
    public partial class DropActionFromPermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Permissions");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "RoleHasPermissions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "RoleHasPermissions");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Permissions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
