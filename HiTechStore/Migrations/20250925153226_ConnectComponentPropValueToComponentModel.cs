using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class ConnectComponentPropValueToComponentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComponentPropertyValue",
                columns: table => new
                {
                    ComponentPropertyValueId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyId = table.Column<int>(type: "integer", nullable: true),
                    ComponentModelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentPropertyValue", x => x.ComponentPropertyValueId);
                    table.ForeignKey(
                        name: "FK_ComponentPropertyValue_ComponentModel_ComponentModelId",
                        column: x => x.ComponentModelId,
                        principalTable: "ComponentModel",
                        principalColumn: "ComponentModelId");
                    table.ForeignKey(
                        name: "FK_ComponentPropertyValue_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "PropertyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValue_ComponentPropertyValueId",
                table: "PropertyValue",
                column: "ComponentPropertyValueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentPropertyValue_ComponentModelId",
                table: "ComponentPropertyValue",
                column: "ComponentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentPropertyValue_PropertyId",
                table: "ComponentPropertyValue",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyValue_ComponentPropertyValue_ComponentPropertyValue~",
                table: "PropertyValue",
                column: "ComponentPropertyValueId",
                principalTable: "ComponentPropertyValue",
                principalColumn: "ComponentPropertyValueId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyValue_ComponentPropertyValue_ComponentPropertyValue~",
                table: "PropertyValue");

            migrationBuilder.DropTable(
                name: "ComponentPropertyValue");

            migrationBuilder.DropIndex(
                name: "IX_PropertyValue_ComponentPropertyValueId",
                table: "PropertyValue");
        }
    }
}
