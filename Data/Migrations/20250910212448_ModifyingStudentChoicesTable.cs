using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyingStudentChoicesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentChoices_Questions_QuestionId",
                table: "StudentChoices");

            migrationBuilder.DropIndex(
                name: "IX_StudentChoices_QuestionId",
                table: "StudentChoices");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "StudentChoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionId",
                table: "StudentChoices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentChoices_QuestionId",
                table: "StudentChoices",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentChoices_Questions_QuestionId",
                table: "StudentChoices",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
