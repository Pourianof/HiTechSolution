using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeletionOnBrandToBrandModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandModel_Brand_BrandId",
                table: "BrandModel");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandModel_Brand_BrandId",
                table: "BrandModel",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandModel_Brand_BrandId",
                table: "BrandModel");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandModel_Brand_BrandId",
                table: "BrandModel",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "BrandId");
        }
    }
}
