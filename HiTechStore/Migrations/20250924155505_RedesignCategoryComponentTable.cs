using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class RedesignCategoryComponentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPropertyValue_CategoryProperty_PropertyId",
                table: "ProductPropertyValue");

            migrationBuilder.DropTable(
                name: "CategoryProperty");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ProductPropertyValue");

            migrationBuilder.AddColumn<int>(
                name: "ProductPropertyValue",
                table: "ProductPropertyValue",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComponentType",
                columns: table => new
                {
                    ComponentTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentType", x => x.ComponentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "PropertyValue",
                columns: table => new
                {
                    PropertyValueId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentPropertyValueId = table.Column<int>(type: "integer", nullable: false),
                    ValueString = table.Column<string>(type: "text", nullable: true),
                    ValueNumber = table.Column<double>(type: "double precision", nullable: true),
                    ValueBoolean = table.Column<bool>(type: "boolean", nullable: true),
                    ValueDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValueReferenceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyValue", x => x.PropertyValueId);
                });

            migrationBuilder.CreateTable(
                name: "CategoryComponent",
                columns: table => new
                {
                    CategoryComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    ComponentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryComponent", x => x.CategoryComponentId);
                    table.ForeignKey(
                        name: "FK_CategoryComponent_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryComponent_ComponentType_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "ComponentType",
                        principalColumn: "ComponentTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentModel",
                columns: table => new
                {
                    ComponentModelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentTypeId = table.Column<int>(type: "integer", nullable: true),
                    BrandModelId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentModel", x => x.ComponentModelId);
                    table.ForeignKey(
                        name: "FK_ComponentModel_BrandModel_BrandModelId",
                        column: x => x.BrandModelId,
                        principalTable: "BrandModel",
                        principalColumn: "BrandModelId");
                    table.ForeignKey(
                        name: "FK_ComponentModel_ComponentType_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentType",
                        principalColumn: "ComponentTypeId");
                    table.ForeignKey(
                        name: "FK_ComponentModel_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    propertyType = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ComponentTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_Property_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Property_ComponentType_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentType",
                        principalColumn: "ComponentTypeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPropertyValue_ProductPropertyValue",
                table: "ProductPropertyValue",
                column: "ProductPropertyValue");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryComponent_CategoryId",
                table: "CategoryComponent",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryComponent_ComponentId",
                table: "CategoryComponent",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentModel_BrandModelId",
                table: "ComponentModel",
                column: "BrandModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentModel_ComponentTypeId",
                table: "ComponentModel",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentModel_ProductId",
                table: "ComponentModel",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_CategoryId",
                table: "Property",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_ComponentTypeId",
                table: "Property",
                column: "ComponentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPropertyValue_PropertyValue_ProductPropertyValue",
                table: "ProductPropertyValue",
                column: "ProductPropertyValue",
                principalTable: "PropertyValue",
                principalColumn: "PropertyValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPropertyValue_Property_PropertyId",
                table: "ProductPropertyValue",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPropertyValue_PropertyValue_ProductPropertyValue",
                table: "ProductPropertyValue");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPropertyValue_Property_PropertyId",
                table: "ProductPropertyValue");

            migrationBuilder.DropTable(
                name: "CategoryComponent");

            migrationBuilder.DropTable(
                name: "ComponentModel");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "PropertyValue");

            migrationBuilder.DropTable(
                name: "ComponentType");

            migrationBuilder.DropIndex(
                name: "IX_ProductPropertyValue_ProductPropertyValue",
                table: "ProductPropertyValue");

            migrationBuilder.DropColumn(
                name: "ProductPropertyValue",
                table: "ProductPropertyValue");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ProductPropertyValue",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryProperty",
                columns: table => new
                {
                    CategoryPropertyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProperty", x => x.CategoryPropertyId);
                    table.ForeignKey(
                        name: "FK_CategoryProperty_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProperty_CategoryId",
                table: "CategoryProperty",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPropertyValue_CategoryProperty_PropertyId",
                table: "ProductPropertyValue",
                column: "PropertyId",
                principalTable: "CategoryProperty",
                principalColumn: "CategoryPropertyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
