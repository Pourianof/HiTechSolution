using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeletionPropertiesForComponentTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Property_ComponentType_ComponentTypeId",
                table: "Property");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_ComponentType_ComponentTypeId",
                table: "Property",
                column: "ComponentTypeId",
                principalTable: "ComponentType",
                principalColumn: "ComponentTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Property_ComponentType_ComponentTypeId",
                table: "Property");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_ComponentType_ComponentTypeId",
                table: "Property",
                column: "ComponentTypeId",
                principalTable: "ComponentType",
                principalColumn: "ComponentTypeId");
        }
    }
}
