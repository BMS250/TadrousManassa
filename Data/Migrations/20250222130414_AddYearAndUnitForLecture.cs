using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddYearAndUnitForLecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Lectures",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Lectures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Lectures");
        }
    }
}
