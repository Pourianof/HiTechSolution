using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheCategoryComponentTableKey : Migration
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryComponent",
                table: "CategoryComponent");

            migrationBuilder.DropIndex(
                name: "IX_CategoryComponent_ComponentId",
                table: "CategoryComponent");

            migrationBuilder.DropColumn(
                name: "CategoryComponentId",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryComponent",
                table: "CategoryComponent",
                columns: new[] { "ComponentId", "CategoryId" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_Categories_CategoryId",
                table: "CategoryComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryComponent_ComponentType_ComponentId",
                table: "CategoryComponent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryComponent",
                table: "CategoryComponent");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategoryComponent",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "CategoryComponent",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CategoryComponentId",
                table: "CategoryComponent",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryComponent",
                table: "CategoryComponent",
                column: "CategoryComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryComponent_ComponentId",
                table: "CategoryComponent",
                column: "ComponentId");

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
    }
}
