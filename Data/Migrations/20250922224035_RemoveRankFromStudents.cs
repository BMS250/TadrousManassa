using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRankFromStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Students");

            migrationBuilder.AddColumn<double>(
                name: "TotalScore",
                table: "Students",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
