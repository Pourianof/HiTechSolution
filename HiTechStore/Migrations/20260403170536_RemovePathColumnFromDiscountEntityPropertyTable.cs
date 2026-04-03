using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class RemovePathColumnFromDiscountEntityPropertyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiscountEntityProperties_Path",
                table: "DiscountEntityProperties");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "DiscountEntityProperties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "DiscountEntityProperties",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountEntityProperties_Path",
                table: "DiscountEntityProperties",
                column: "Path",
                unique: true);
        }
    }
}
