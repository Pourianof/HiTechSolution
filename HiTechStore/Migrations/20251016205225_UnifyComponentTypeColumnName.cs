using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class UnifyComponentTypeColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentId",
                table: "CategoryComponent");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "CategoryComponent",
                newName: "ComponentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentTypeId",
                table: "CategoryComponent",
                column: "ComponentTypeId",
                principalTable: "ComponentType",
                principalColumn: "ComponentTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentTypeId",
                table: "CategoryComponent");

            migrationBuilder.RenameColumn(
                name: "ComponentTypeId",
                table: "CategoryComponent",
                newName: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentId",
                table: "CategoryComponent",
                column: "ComponentId",
                principalTable: "ComponentType",
                principalColumn: "ComponentTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
