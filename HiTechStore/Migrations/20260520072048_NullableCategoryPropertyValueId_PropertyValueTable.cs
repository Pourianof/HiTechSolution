using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class NullableCategoryPropertyValueId_PropertyValueTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariation_Products_ProductId",
                table: "ProductVariation");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyValue_ComponentPropertyValue_ComponentPropertyValue~",
                table: "PropertyValue");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentPropertyValueId",
                table: "PropertyValue",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductVariation",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariation_Products_ProductId",
                table: "ProductVariation",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyValue_ComponentPropertyValue_ComponentPropertyValue~",
                table: "PropertyValue",
                column: "ComponentPropertyValueId",
                principalTable: "ComponentPropertyValue",
                principalColumn: "ComponentPropertyValueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariation_Products_ProductId",
                table: "ProductVariation");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyValue_ComponentPropertyValue_ComponentPropertyValue~",
                table: "PropertyValue");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentPropertyValueId",
                table: "PropertyValue",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductVariation",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariation_Products_ProductId",
                table: "ProductVariation",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyValue_ComponentPropertyValue_ComponentPropertyValue~",
                table: "PropertyValue",
                column: "ComponentPropertyValueId",
                principalTable: "ComponentPropertyValue",
                principalColumn: "ComponentPropertyValueId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
