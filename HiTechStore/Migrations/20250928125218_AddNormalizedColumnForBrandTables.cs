using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedColumnForBrandTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "BrandModel",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Brand",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandModel_NormalizedName",
                table: "BrandModel",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brand_NormalizedName",
                table: "Brand",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrandModel_NormalizedName",
                table: "BrandModel");

            migrationBuilder.DropIndex(
                name: "IX_Brand_NormalizedName",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "BrandModel");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Brand");
        }
    }
}
