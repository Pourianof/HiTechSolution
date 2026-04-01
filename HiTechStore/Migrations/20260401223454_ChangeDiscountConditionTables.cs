using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDiscountConditionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountCondition");

            migrationBuilder.DropTable(
                name: "DiscountConditionGroup");

            migrationBuilder.CreateTable(
                name: "ConditionComponents",
                columns: table => new
                {
                    ConditionComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    PropertyDiscountEntityPropertyId = table.Column<int>(type: "integer", nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    DiscountRuleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionComponents", x => x.ConditionComponentId);
                    table.ForeignKey(
                        name: "FK_ConditionComponents_ConditionComponents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ConditionComponents",
                        principalColumn: "ConditionComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConditionComponents_DiscountEntityProperties_PropertyDiscou~",
                        column: x => x.PropertyDiscountEntityPropertyId,
                        principalTable: "DiscountEntityProperties",
                        principalColumn: "DiscountEntityPropertyId");
                    table.ForeignKey(
                        name: "FK_ConditionComponents_DiscountRules_DiscountRuleId",
                        column: x => x.DiscountRuleId,
                        principalTable: "DiscountRules",
                        principalColumn: "DiscountRuleId");
                });

            migrationBuilder.CreateTable(
                name: "ConditionMethod",
                columns: table => new
                {
                    ConditionMethodId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ReturnType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionMethod", x => x.ConditionMethodId);
                });

            migrationBuilder.CreateTable(
                name: "ConditionLambda",
                columns: table => new
                {
                    ConditionLambdaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MethodConditionMethodId = table.Column<int>(type: "integer", nullable: true),
                    OwnerConditionId = table.Column<int>(type: "integer", nullable: false),
                    BodyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionLambda", x => x.ConditionLambdaId);
                    table.ForeignKey(
                        name: "FK_ConditionLambda_ConditionComponents_BodyId",
                        column: x => x.BodyId,
                        principalTable: "ConditionComponents",
                        principalColumn: "ConditionComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConditionLambda_ConditionComponents_OwnerConditionId",
                        column: x => x.OwnerConditionId,
                        principalTable: "ConditionComponents",
                        principalColumn: "ConditionComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConditionLambda_ConditionMethod_MethodConditionMethodId",
                        column: x => x.MethodConditionMethodId,
                        principalTable: "ConditionMethod",
                        principalColumn: "ConditionMethodId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConditionComponents_DiscountRuleId",
                table: "ConditionComponents",
                column: "DiscountRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionComponents_ParentId",
                table: "ConditionComponents",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionComponents_PropertyDiscountEntityPropertyId",
                table: "ConditionComponents",
                column: "PropertyDiscountEntityPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionLambda_BodyId",
                table: "ConditionLambda",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionLambda_MethodConditionMethodId",
                table: "ConditionLambda",
                column: "MethodConditionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionLambda_OwnerConditionId",
                table: "ConditionLambda",
                column: "OwnerConditionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConditionLambda");

            migrationBuilder.DropTable(
                name: "ConditionComponents");

            migrationBuilder.DropTable(
                name: "ConditionMethod");

            migrationBuilder.CreateTable(
                name: "DiscountConditionGroup",
                columns: table => new
                {
                    DiscountConditionGroupId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscountRuleId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountConditionGroup", x => x.DiscountConditionGroupId);
                    table.ForeignKey(
                        name: "FK_DiscountConditionGroup_DiscountRules_DiscountRuleId",
                        column: x => x.DiscountRuleId,
                        principalTable: "DiscountRules",
                        principalColumn: "DiscountRuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCondition",
                columns: table => new
                {
                    DiscountConditionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityPropertyId = table.Column<int>(type: "integer", nullable: false),
                    DiscountConditionGroupId = table.Column<int>(type: "integer", nullable: true),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCondition", x => x.DiscountConditionId);
                    table.ForeignKey(
                        name: "FK_DiscountCondition_DiscountConditionGroup_DiscountConditionG~",
                        column: x => x.DiscountConditionGroupId,
                        principalTable: "DiscountConditionGroup",
                        principalColumn: "DiscountConditionGroupId");
                    table.ForeignKey(
                        name: "FK_DiscountCondition_DiscountEntityProperties_EntityPropertyId",
                        column: x => x.EntityPropertyId,
                        principalTable: "DiscountEntityProperties",
                        principalColumn: "DiscountEntityPropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCondition_DiscountConditionGroupId",
                table: "DiscountCondition",
                column: "DiscountConditionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCondition_EntityPropertyId",
                table: "DiscountCondition",
                column: "EntityPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountConditionGroup_DiscountRuleId",
                table: "DiscountConditionGroup",
                column: "DiscountRuleId");
        }
    }
}
