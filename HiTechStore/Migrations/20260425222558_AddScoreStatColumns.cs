using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreStatColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageScore",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreCounts",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@$"
                UPDATE public.""Products""
                SET 
                    ""ScoreCounts"" = (
                        SELECT COUNT(s.""ProductScoreId"") 
                        FROM public.""ProductScores"" s 
                        WHERE s.""ProductId"" = public.""Products"".""ProductId""
                    ),
                    ""AverageScore"" = (
                        SELECT AVG(CAST(s.""Score"" AS FLOAT)) 
                        FROM public.""ProductScores"" s 
                        WHERE s.""ProductId"" = public.""Products"".""ProductId""
                    );
               ;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageScore",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ScoreCounts",
                table: "Products");
        }
    }
}
