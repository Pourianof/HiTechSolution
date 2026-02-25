using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorColumnToDiscountCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "DiscountCodes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCodes_CreatorId",
                table: "DiscountCodes",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCodes_AspNetUsers_CreatorId",
                table: "DiscountCodes",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCodes_AspNetUsers_CreatorId",
                table: "DiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_DiscountCodes_CreatorId",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "DiscountCodes");
        }
    }
}
