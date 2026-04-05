using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToDiscountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountRules_DiscountCodes_DiscountCodeId",
                table: "DiscountRules");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DiscountCodes_DiscountCodeId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DiscountCodes");

            migrationBuilder.RenameColumn(
                name: "DiscountCodeId",
                table: "Orders",
                newName: "DiscountCodeDiscountId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DiscountCodeId",
                table: "Orders",
                newName: "IX_Orders_DiscountCodeDiscountId");

            migrationBuilder.RenameColumn(
                name: "DiscountCodeId",
                table: "DiscountRules",
                newName: "DiscountId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountRules_DiscountCodeId",
                table: "DiscountRules",
                newName: "IX_DiscountRules_DiscountId");

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    DiscountId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Code = table.Column<string>(type: "text", nullable: true),
                    IsDiscountCode = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatorId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.DiscountId);
                    table.ForeignKey(
                        name: "FK_Discounts_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CreatorId",
                table: "Discounts",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountRules_Discounts_DiscountId",
                table: "DiscountRules",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "DiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Discounts_DiscountCodeDiscountId",
                table: "Orders",
                column: "DiscountCodeDiscountId",
                principalTable: "Discounts",
                principalColumn: "DiscountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountRules_Discounts_DiscountId",
                table: "DiscountRules");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Discounts_DiscountCodeDiscountId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.RenameColumn(
                name: "DiscountCodeDiscountId",
                table: "Orders",
                newName: "DiscountCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DiscountCodeDiscountId",
                table: "Orders",
                newName: "IX_Orders_DiscountCodeId");

            migrationBuilder.RenameColumn(
                name: "DiscountId",
                table: "DiscountRules",
                newName: "DiscountCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountRules_DiscountId",
                table: "DiscountRules",
                newName: "IX_DiscountRules_DiscountCodeId");

            migrationBuilder.CreateTable(
                name: "DiscountCodes",
                columns: table => new
                {
                    DiscountCodeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCodes", x => x.DiscountCodeId);
                    table.ForeignKey(
                        name: "FK_DiscountCodes_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCodes_CreatorId",
                table: "DiscountCodes",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountRules_DiscountCodes_DiscountCodeId",
                table: "DiscountRules",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "DiscountCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DiscountCodes_DiscountCodeId",
                table: "Orders",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "DiscountCodeId");
        }
    }
}
