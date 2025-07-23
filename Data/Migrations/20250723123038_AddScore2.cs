using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScore2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "StudentQuizzes",
                newName: "Score2");

            migrationBuilder.AddColumn<float>(
                name: "Score1",
                table: "StudentQuizzes",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score1",
                table: "StudentQuizzes");

            migrationBuilder.RenameColumn(
                name: "Score2",
                table: "StudentQuizzes",
                newName: "Score");
        }
    }
}
