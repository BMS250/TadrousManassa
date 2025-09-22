using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveSheetPathToVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SheetPath",
                table: "Lectures");

            migrationBuilder.AddColumn<string>(
                name: "SheetPath",
                table: "Videos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SheetPath",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "SheetPath",
                table: "Lectures",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
