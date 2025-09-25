using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class MakeCategoryComponentsTableForeignKeyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_Categories_CategoryId",
                table: "CategoryComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentId",
                table: "CategoryComponent");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "CategoryComponent",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategoryComponent",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryComponent_Categories_CategoryId",
                table: "CategoryComponent",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentId",
                table: "CategoryComponent",
                column: "ComponentId",
                principalTable: "ComponentType",
                principalColumn: "ComponentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_Categories_CategoryId",
                table: "CategoryComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentId",
                table: "CategoryComponent");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "CategoryComponent",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategoryComponent",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryComponent_Categories_CategoryId",
                table: "CategoryComponent",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

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
