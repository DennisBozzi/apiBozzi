﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiBozzi.Migrations
{
    /// <inheritdoc />
    public partial class feliciano_bozzi_adding_apartmenttypeenum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Apartments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Apartments");
        }
    }
}
