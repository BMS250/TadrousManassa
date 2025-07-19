using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnsureRestrictedDeletionForVideoQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Videos_VideoId",
                table: "Quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Videos_VideoId",
                table: "Quizzes",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Videos_VideoId",
                table: "Quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Videos_VideoId",
                table: "Quizzes",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
