using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeAnswerIdNotNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Choices_AnswerId",
                table: "Questions");

            migrationBuilder.AlterColumn<string>(
                name: "AnswerId",
                table: "Questions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Choices_AnswerId",
                table: "Questions",
                column: "AnswerId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Choices_AnswerId",
                table: "Questions");

            migrationBuilder.AlterColumn<string>(
                name: "AnswerId",
                table: "Questions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Choices_AnswerId",
                table: "Questions",
                column: "AnswerId",
                principalTable: "Choices",
                principalColumn: "Id");
        }
    }
}
