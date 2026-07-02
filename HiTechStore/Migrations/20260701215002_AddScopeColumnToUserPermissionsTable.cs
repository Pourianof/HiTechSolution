using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddScopeColumnToUserPermissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "UserPermission",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "PermissionAudits",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "UserPermission");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "PermissionAudits");
        }
    }
}
