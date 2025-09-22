using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionIdInStudentChoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmissionId",
                table: "StudentChoices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentChoices_SubmissionId",
                table: "StudentChoices",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentChoices_Submissions_SubmissionId",
                table: "StudentChoices",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentChoices_Submissions_SubmissionId",
                table: "StudentChoices");

            migrationBuilder.DropIndex(
                name: "IX_StudentChoices_SubmissionId",
                table: "StudentChoices");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "StudentChoices");
        }
    }
}
