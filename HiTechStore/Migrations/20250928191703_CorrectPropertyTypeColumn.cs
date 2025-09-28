using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class CorrectPropertyTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "propertyType",
                table: "Property",
                newName: "PropertyType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PropertyType",
                table: "Property",
                newName: "propertyType");
        }
    }
}
