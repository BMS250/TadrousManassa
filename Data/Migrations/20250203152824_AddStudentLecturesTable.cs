using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TadrousManassa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentLecturesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Lectures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Lectures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudentLectures",
                columns: table => new
                {
                    LectureId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLectures", x => new { x.StudentId, x.LectureId });
                    table.ForeignKey(
                        name: "FK_StudentLectures_Lectures_LectureId",
                        column: x => x.LectureId,
                        principalTable: "Lectures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLectures_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLectures_LectureId",
                table: "StudentLectures",
                column: "LectureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentLectures");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Lectures");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Students",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Students",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    HashPassword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });
        }
    }
}
