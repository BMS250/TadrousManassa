using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class PutRemainingAttemptsInStudentQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumOfRemainingAttempts",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "NumOfRemainingAttempts",
                table: "StudentQuizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumOfRemainingAttempts",
                table: "StudentQuizzes");

            migrationBuilder.AddColumn<int>(
                name: "NumOfRemainingAttempts",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
