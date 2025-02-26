using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsedThisYearAnnotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsedThisYear",
                table: "Lectures",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedThisYear",
                table: "Lectures");
        }
    }
}
