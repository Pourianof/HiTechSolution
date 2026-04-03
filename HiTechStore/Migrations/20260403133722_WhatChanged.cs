using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiTechStore.Migrations
{
    /// <inheritdoc />
    public partial class WhatChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConditionLambda_ConditionMethod_MethodConditionMethodId",
                table: "ConditionLambda");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ClientId",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConditionMethod",
                table: "ConditionMethod");

            migrationBuilder.RenameTable(
                name: "ConditionMethod",
                newName: "ConditionMethods");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConditionMethods",
                table: "ConditionMethods",
                column: "ConditionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ClientId",
                table: "Carts",
                column: "ClientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConditionLambda_ConditionMethods_MethodConditionMethodId",
                table: "ConditionLambda",
                column: "MethodConditionMethodId",
                principalTable: "ConditionMethods",
                principalColumn: "ConditionMethodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConditionLambda_ConditionMethods_MethodConditionMethodId",
                table: "ConditionLambda");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ClientId",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConditionMethods",
                table: "ConditionMethods");

            migrationBuilder.RenameTable(
                name: "ConditionMethods",
                newName: "ConditionMethod");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConditionMethod",
                table: "ConditionMethod",
                column: "ConditionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ClientId",
                table: "Carts",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConditionLambda_ConditionMethod_MethodConditionMethodId",
                table: "ConditionLambda",
                column: "MethodConditionMethodId",
                principalTable: "ConditionMethod",
                principalColumn: "ConditionMethodId");
        }
    }
}
