using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountCodes",
                columns: table => new
                {
                    DiscountCodeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCodes", x => x.DiscountCodeId);
                });

            migrationBuilder.CreateTable(
                name: "DiscountEntities",
                columns: table => new
                {
                    DiscountEntityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountEntities", x => x.DiscountEntityId);
                });

            migrationBuilder.CreateTable(
                name: "DiscountRules",
                columns: table => new
                {
                    DiscountRuleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DiscountAction_Type = table.Column<int>(type: "integer", nullable: true),
                    DiscountAction_Value = table.Column<decimal>(type: "numeric", nullable: true),
                    DiscountCodeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountRules", x => x.DiscountRuleId);
                    table.ForeignKey(
                        name: "FK_DiscountRules_DiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "DiscountCodes",
                        principalColumn: "DiscountCodeId");
                });

            migrationBuilder.CreateTable(
                name: "DiscountEntityProperties",
                columns: table => new
                {
                    DiscountEntityPropertyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    SubEntityId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountEntityProperties", x => x.DiscountEntityPropertyId);
                    table.ForeignKey(
                        name: "FK_DiscountEntityProperties_DiscountEntities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "DiscountEntities",
                        principalColumn: "DiscountEntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountEntityProperties_DiscountEntities_SubEntityId",
                        column: x => x.SubEntityId,
                        principalTable: "DiscountEntities",
                        principalColumn: "DiscountEntityId");
                });

            migrationBuilder.CreateTable(
                name: "DiscountConditionGroup",
                columns: table => new
                {
                    DiscountConditionGroupId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscountRuleId = table.Column<int>(type: "integer", nullable: false)
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
                    EntityPropertyDiscountEntityPropertyId = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DiscountConditionGroupId = table.Column<int>(type: "integer", nullable: true)
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
                        name: "FK_DiscountCondition_DiscountEntityProperties_EntityPropertyDi~",
                        column: x => x.EntityPropertyDiscountEntityPropertyId,
                        principalTable: "DiscountEntityProperties",
                        principalColumn: "DiscountEntityPropertyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCondition_DiscountConditionGroupId",
                table: "DiscountCondition",
                column: "DiscountConditionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCondition_EntityPropertyDiscountEntityPropertyId",
                table: "DiscountCondition",
                column: "EntityPropertyDiscountEntityPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountConditionGroup_DiscountRuleId",
                table: "DiscountConditionGroup",
                column: "DiscountRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountEntities_Name",
                table: "DiscountEntities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountEntityProperties_EntityId_Name",
                table: "DiscountEntityProperties",
                columns: new[] { "EntityId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountEntityProperties_SubEntityId",
                table: "DiscountEntityProperties",
                column: "SubEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountRules_DiscountCodeId",
                table: "DiscountRules",
                column: "DiscountCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountCondition");

            migrationBuilder.DropTable(
                name: "DiscountConditionGroup");

            migrationBuilder.DropTable(
                name: "DiscountEntityProperties");

            migrationBuilder.DropTable(
                name: "DiscountRules");

            migrationBuilder.DropTable(
                name: "DiscountEntities");

            migrationBuilder.DropTable(
                name: "DiscountCodes");
        }
    }
}
