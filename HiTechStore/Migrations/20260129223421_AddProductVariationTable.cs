using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Products_ProductId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedia_Products_ProductId",
                table: "ProductMedia");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderItem",
                newName: "ProductVariationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_ProductId",
                table: "OrderItem",
                newName: "IX_OrderItem_ProductVariationId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "CartItem",
                newName: "ProductVariationId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_ProductId",
                table: "CartItem",
                newName: "IX_CartItem_ProductVariationId");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "ProductMedia",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Color", x => x.ColorId);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariation",
                columns: table => new
                {
                    ProductVariationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    ColorId = table.Column<int>(type: "integer", nullable: false),
                    Inventory = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariation", x => x.ProductVariationId);
                    table.ForeignKey(
                        name: "FK_ProductVariation_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "ColorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariation_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_ProductVariationId",
                table: "ProductMedia",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariation_ColorId",
                table: "ProductVariation",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariation_ProductId",
                table: "ProductVariation",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_ProductVariation_ProductVariationId",
                table: "CartItem",
                column: "ProductVariationId",
                principalTable: "ProductVariation",
                principalColumn: "ProductVariationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_ProductVariation_ProductVariationId",
                table: "OrderItem",
                column: "ProductVariationId",
                principalTable: "ProductVariation",
                principalColumn: "ProductVariationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedia_ProductVariation_ProductVariationId",
                table: "ProductMedia",
                column: "ProductVariationId",
                principalTable: "ProductVariation",
                principalColumn: "ProductVariationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_ProductVariation_ProductVariationId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_ProductVariation_ProductVariationId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedia_ProductVariation_ProductVariationId",
                table: "ProductMedia");

            migrationBuilder.DropTable(
                name: "ProductVariation");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedia_ProductVariationId",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "ProductMedia");

            migrationBuilder.RenameColumn(
                name: "ProductVariationId",
                table: "OrderItem",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_ProductVariationId",
                table: "OrderItem",
                newName: "IX_OrderItem_ProductId");

            migrationBuilder.RenameColumn(
                name: "ProductVariationId",
                table: "CartItem",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_ProductVariationId",
                table: "CartItem",
                newName: "IX_CartItem_ProductId");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Products",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Products_ProductId",
                table: "CartItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedia_Products_ProductId",
                table: "ProductMedia",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
