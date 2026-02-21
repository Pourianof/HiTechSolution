using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityPropertyIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCondition_DiscountEntityProperties_EntityPropertyDi~",
                table: "DiscountCondition");

            migrationBuilder.DropIndex(
                name: "IX_DiscountCondition_EntityPropertyDiscountEntityPropertyId",
                table: "DiscountCondition");

            migrationBuilder.DropColumn(
                name: "EntityPropertyDiscountEntityPropertyId",
                table: "DiscountCondition");

            migrationBuilder.AddColumn<int>(
                name: "EntityPropertyId",
                table: "DiscountCondition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCondition_EntityPropertyId",
                table: "DiscountCondition",
                column: "EntityPropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCondition_DiscountEntityProperties_EntityPropertyId",
                table: "DiscountCondition",
                column: "EntityPropertyId",
                principalTable: "DiscountEntityProperties",
                principalColumn: "DiscountEntityPropertyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCondition_DiscountEntityProperties_EntityPropertyId",
                table: "DiscountCondition");

            migrationBuilder.DropIndex(
                name: "IX_DiscountCondition_EntityPropertyId",
                table: "DiscountCondition");

            migrationBuilder.DropColumn(
                name: "EntityPropertyId",
                table: "DiscountCondition");

            migrationBuilder.AddColumn<int>(
                name: "EntityPropertyDiscountEntityPropertyId",
                table: "DiscountCondition",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCondition_EntityPropertyDiscountEntityPropertyId",
                table: "DiscountCondition",
                column: "EntityPropertyDiscountEntityPropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCondition_DiscountEntityProperties_EntityPropertyDi~",
                table: "DiscountCondition",
                column: "EntityPropertyDiscountEntityPropertyId",
                principalTable: "DiscountEntityProperties",
                principalColumn: "DiscountEntityPropertyId");
        }
    }
}
