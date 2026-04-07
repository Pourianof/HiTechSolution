using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class CorrectConditionComponentColumnInDiscountRuleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConditionComponents_DiscountRules_DiscountRuleId",
                table: "ConditionComponents");

            migrationBuilder.DropIndex(
                name: "IX_ConditionComponents_DiscountRuleId",
                table: "ConditionComponents");

            migrationBuilder.DropColumn(
                name: "DiscountRuleId",
                table: "ConditionComponents");

            migrationBuilder.AddColumn<int>(
                name: "ConditionTreeConditionComponentId",
                table: "DiscountRules",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountRules_ConditionTreeConditionComponentId",
                table: "DiscountRules",
                column: "ConditionTreeConditionComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountRules_ConditionComponents_ConditionTreeConditionCom~",
                table: "DiscountRules",
                column: "ConditionTreeConditionComponentId",
                principalTable: "ConditionComponents",
                principalColumn: "ConditionComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountRules_ConditionComponents_ConditionTreeConditionCom~",
                table: "DiscountRules");

            migrationBuilder.DropIndex(
                name: "IX_DiscountRules_ConditionTreeConditionComponentId",
                table: "DiscountRules");

            migrationBuilder.DropColumn(
                name: "ConditionTreeConditionComponentId",
                table: "DiscountRules");

            migrationBuilder.AddColumn<int>(
                name: "DiscountRuleId",
                table: "ConditionComponents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConditionComponents_DiscountRuleId",
                table: "ConditionComponents",
                column: "DiscountRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConditionComponents_DiscountRules_DiscountRuleId",
                table: "ConditionComponents",
                column: "DiscountRuleId",
                principalTable: "DiscountRules",
                principalColumn: "DiscountRuleId");
        }
    }
}
