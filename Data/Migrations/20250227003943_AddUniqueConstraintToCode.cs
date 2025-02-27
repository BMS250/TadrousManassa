using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentLectures_Code_LectureId",
                table: "StudentLectures");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLectures_Code",
                table: "StudentLectures",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentLectures_Code",
                table: "StudentLectures");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLectures_Code_LectureId",
                table: "StudentLectures",
                columns: new[] { "Code", "LectureId" },
                unique: true);
        }
    }
}
