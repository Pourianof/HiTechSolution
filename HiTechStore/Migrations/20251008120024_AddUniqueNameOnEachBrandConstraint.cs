using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueNameOnEachBrandConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrandModel_NormalizedName",
                table: "BrandModel");

            migrationBuilder.AlterColumn<int>(
                name: "BrandId",
                table: "BrandModel",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandModel_NormalizedName_BrandId",
                table: "BrandModel",
                columns: new[] { "NormalizedName", "BrandId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrandModel_NormalizedName_BrandId",
                table: "BrandModel");

            migrationBuilder.AlterColumn<int>(
                name: "BrandId",
                table: "BrandModel",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_BrandModel_NormalizedName",
                table: "BrandModel",
                column: "NormalizedName",
                unique: true);
        }
    }
}
