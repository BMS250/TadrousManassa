using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingIdInStudentLectureTableAndMakingItPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLectures",
                table: "StudentLectures");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StudentLectures",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLectures",
                table: "StudentLectures",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLectures_StudentId",
                table: "StudentLectures",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLectures",
                table: "StudentLectures");

            migrationBuilder.DropIndex(
                name: "IX_StudentLectures_StudentId",
                table: "StudentLectures");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudentLectures");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLectures",
                table: "StudentLectures",
                columns: new[] { "StudentId", "LectureId" });
        }
    }
}
