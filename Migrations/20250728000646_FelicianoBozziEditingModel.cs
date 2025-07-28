using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiBozzi.Migrations
{
    /// <inheritdoc />
    public partial class FelicianoBozziEditingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Tenants_ResponsibleTenantId",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "ResponsibleTenantId",
                table: "Apartments",
                newName: "ResponsibleId");

            migrationBuilder.RenameIndex(
                name: "IX_Apartments_ResponsibleTenantId",
                table: "Apartments",
                newName: "IX_Apartments_ResponsibleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Tenants_ResponsibleId",
                table: "Apartments",
                column: "ResponsibleId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Tenants_ResponsibleId",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "ResponsibleId",
                table: "Apartments",
                newName: "ResponsibleTenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Apartments_ResponsibleId",
                table: "Apartments",
                newName: "IX_Apartments_ResponsibleTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Tenants_ResponsibleTenantId",
                table: "Apartments",
                column: "ResponsibleTenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }
    }
}
