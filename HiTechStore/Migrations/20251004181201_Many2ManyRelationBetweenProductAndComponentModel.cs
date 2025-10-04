using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class Many2ManyRelationBetweenProductAndComponentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentModel_Products_ProductId",
                table: "ComponentModel");

            migrationBuilder.DropIndex(
                name: "IX_ComponentModel_ProductId",
                table: "ComponentModel");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ComponentModel");

            migrationBuilder.CreateTable(
                name: "ProductComponents",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ComponentModelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductComponents", x => new { x.ProductId, x.ComponentModelId });
                    table.ForeignKey(
                        name: "FK_ProductComponents_ComponentModel_ComponentModelId",
                        column: x => x.ComponentModelId,
                        principalTable: "ComponentModel",
                        principalColumn: "ComponentModelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductComponents_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductComponents_ComponentModelId",
                table: "ProductComponents",
                column: "ComponentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductComponents_ProductId_ComponentModelId",
                table: "ProductComponents",
                columns: new[] { "ProductId", "ComponentModelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductComponents");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ComponentModel",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentModel_ProductId",
                table: "ComponentModel",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentModel_Products_ProductId",
                table: "ComponentModel",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }
    }
}
