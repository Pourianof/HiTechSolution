using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class MakeComponentModelUniqueOnBrandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentPropertyValue_Property_PropertyId",
                table: "ComponentPropertyValue");

            migrationBuilder.DropIndex(
                name: "IX_ComponentModel_ComponentTypeId",
                table: "ComponentModel");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "ComponentPropertyValue",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentModel_ComponentTypeId_BrandModelId",
                table: "ComponentModel",
                columns: new[] { "ComponentTypeId", "BrandModelId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentPropertyValue_Property_PropertyId",
                table: "ComponentPropertyValue",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentPropertyValue_Property_PropertyId",
                table: "ComponentPropertyValue");

            migrationBuilder.DropIndex(
                name: "IX_ComponentModel_ComponentTypeId_BrandModelId",
                table: "ComponentModel");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "ComponentPropertyValue",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentModel_ComponentTypeId",
                table: "ComponentModel",
                column: "ComponentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentPropertyValue_Property_PropertyId",
                table: "ComponentPropertyValue",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "PropertyId");
        }
    }
}
