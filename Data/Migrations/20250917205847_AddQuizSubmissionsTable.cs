using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score1",
                table: "StudentQuizzes");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "StudentQuizzes");

            migrationBuilder.DropColumn(
                name: "SubmissionTimeAttempt1",
                table: "StudentQuizzes");

            migrationBuilder.DropColumn(
                name: "SubmissionTimeAttempt2",
                table: "StudentQuizzes");

            migrationBuilder.RenameColumn(
                name: "Score2",
                table: "StudentQuizzes",
                newName: "BestScore");

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentQuizId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_StudentQuizzes_StudentQuizId",
                        column: x => x.StudentQuizId,
                        principalTable: "StudentQuizzes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StudentQuizId",
                table: "Submissions",
                column: "StudentQuizId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.RenameColumn(
                name: "BestScore",
                table: "StudentQuizzes",
                newName: "Score2");

            migrationBuilder.AddColumn<float>(
                name: "Score1",
                table: "StudentQuizzes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "StudentQuizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionTimeAttempt1",
                table: "StudentQuizzes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionTimeAttempt2",
                table: "StudentQuizzes",
                type: "datetime2",
                nullable: true);
        }
    }
}
