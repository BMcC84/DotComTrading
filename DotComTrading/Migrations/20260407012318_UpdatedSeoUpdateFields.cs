using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotComTrading.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSeoUpdateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastSeoUpdate",
                table: "Websites",
                newName: "LastMozUpdate");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMetricUpdate",
                table: "Websites",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMetricUpdate",
                table: "Websites");

            migrationBuilder.RenameColumn(
                name: "LastMozUpdate",
                table: "Websites",
                newName: "LastSeoUpdate");
        }
    }
}
