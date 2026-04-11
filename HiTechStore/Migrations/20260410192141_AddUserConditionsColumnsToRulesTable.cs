using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddUserConditionsColumnsToRulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountRules_ConditionComponents_ConditionTreeConditionCom~",
                table: "DiscountRules");

            migrationBuilder.RenameColumn(
                name: "RawConditionScript",
                table: "DiscountRules",
                newName: "UserRawConditionScript");

            migrationBuilder.RenameColumn(
                name: "ConditionTreeConditionComponentId",
                table: "DiscountRules",
                newName: "UserConditionTreeConditionComponentId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountRules_ConditionTreeConditionComponentId",
                table: "DiscountRules",
                newName: "IX_DiscountRules_UserConditionTreeConditionComponentId");

            migrationBuilder.AddColumn<int>(
                name: "ProductConditionTreeConditionComponentId",
                table: "DiscountRules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductRawConditionScript",
                table: "DiscountRules",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountRules_ProductConditionTreeConditionComponentId",
                table: "DiscountRules",
                column: "ProductConditionTreeConditionComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountRules_ConditionComponents_ProductConditionTreeCondi~",
                table: "DiscountRules",
                column: "ProductConditionTreeConditionComponentId",
                principalTable: "ConditionComponents",
                principalColumn: "ConditionComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountRules_ConditionComponents_UserConditionTreeConditio~",
                table: "DiscountRules",
                column: "UserConditionTreeConditionComponentId",
                principalTable: "ConditionComponents",
                principalColumn: "ConditionComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountRules_ConditionComponents_ProductConditionTreeCondi~",
                table: "DiscountRules");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscountRules_ConditionComponents_UserConditionTreeConditio~",
                table: "DiscountRules");

            migrationBuilder.DropIndex(
                name: "IX_DiscountRules_ProductConditionTreeConditionComponentId",
                table: "DiscountRules");

            migrationBuilder.DropColumn(
                name: "ProductConditionTreeConditionComponentId",
                table: "DiscountRules");

            migrationBuilder.DropColumn(
                name: "ProductRawConditionScript",
                table: "DiscountRules");

            migrationBuilder.RenameColumn(
                name: "UserRawConditionScript",
                table: "DiscountRules",
                newName: "RawConditionScript");

            migrationBuilder.RenameColumn(
                name: "UserConditionTreeConditionComponentId",
                table: "DiscountRules",
                newName: "ConditionTreeConditionComponentId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountRules_UserConditionTreeConditionComponentId",
                table: "DiscountRules",
                newName: "IX_DiscountRules_ConditionTreeConditionComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountRules_ConditionComponents_ConditionTreeConditionCom~",
                table: "DiscountRules",
                column: "ConditionTreeConditionComponentId",
                principalTable: "ConditionComponents",
                principalColumn: "ConditionComponentId");
        }
    }
}
