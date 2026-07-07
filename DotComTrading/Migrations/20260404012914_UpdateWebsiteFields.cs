using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotComTrading.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWebsiteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MozUsed",
                table: "Websites",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MozUsed",
                table: "Websites");
        }
    }
}
