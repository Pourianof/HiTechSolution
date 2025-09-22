using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandModelId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    BrandId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "BrandModel",
                columns: table => new
                {
                    BrandModelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BrandId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandModel", x => x.BrandModelId);
                    table.ForeignKey(
                        name: "FK_BrandModel_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "BrandId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandModelId",
                table: "Products",
                column: "BrandModelId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandModel_BrandId",
                table: "BrandModel",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_BrandModel_BrandModelId",
                table: "Products",
                column: "BrandModelId",
                principalTable: "BrandModel",
                principalColumn: "BrandModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_BrandModel_BrandModelId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "BrandModel");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropIndex(
                name: "IX_Products_BrandModelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BrandModelId",
                table: "Products");
        }
    }
}
