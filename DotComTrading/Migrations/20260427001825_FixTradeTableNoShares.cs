using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotComTrading.Migrations
{
    /// <inheritdoc />
    public partial class FixTradeTableNoShares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NoShares",
                table: "Trades",
                type: "decimal(15,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NoShares",
                table: "Trades",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)");
        }
    }
}
