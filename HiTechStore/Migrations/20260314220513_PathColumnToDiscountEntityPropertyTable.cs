using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class PathColumnToDiscountEntityPropertyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "DiscountEntityProperties",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "DiscountConditionGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountEntityProperties_Path",
                table: "DiscountEntityProperties",
                column: "Path",
                unique: true);

            migrationBuilder.Sql("""
                WITH RECURSIVE recursive_paths AS (
                    SELECT 
                        p."DiscountEntityPropertyId" AS PropertyId,
                        (e."Name" || '/' || p."Name") AS path,
                        p."SubEntityId"
                    FROM public."DiscountEntityProperties" p
                    INNER JOIN public."DiscountEntities" e ON p."EntityId" = e."DiscountEntityId"

                    UNION ALL

                    SELECT 
                        p2."DiscountEntityPropertyId" AS PropertyId,
                        (r."path" || '/' || p2."Name") AS path,
                        p2."SubEntityId"
                    FROM recursive_paths r
                    INNER JOIN public."DiscountEntities" e2 ON e2."DiscountEntityId" = r."SubEntityId"
                    INNER JOIN public."DiscountEntityProperties" p2 ON p2."EntityId" = e2."DiscountEntityId"
                )

                UPDATE public."DiscountEntityProperties" AS p
                SET "Path" = r.path
                FROM recursive_paths AS r
                WHERE p."DiscountEntityPropertyId" = r.PropertyId;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiscountEntityProperties_Path",
                table: "DiscountEntityProperties");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "DiscountEntityProperties");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DiscountConditionGroup");
        }
    }
}
