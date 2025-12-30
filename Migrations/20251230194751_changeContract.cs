using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace apiBozzi.Migrations
{
    /// <inheritdoc />
    public partial class changeContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Tenants_ResponsibleId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ResponsibleId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "ResponsibleId",
                table: "Contracts",
                newName: "TenantId");

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "Contracts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_TenantId",
                table: "Contracts",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Tenants_TenantId",
                table: "Contracts",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Tenants_TenantId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_TenantId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Contracts",
                newName: "ResponsibleId");

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "Contracts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ResponsibleId",
                table: "Contracts",
                column: "ResponsibleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Tenants_ResponsibleId",
                table: "Contracts",
                column: "ResponsibleId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
