using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCoreWebAPI.Sample.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                },
                comment: "班级表");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                },
                comment: "课程表");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "主键"),
                    No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "学生编号"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "学生姓名"),
                    Gender = table.Column<int>(type: "int", nullable: false, comment: "学生性别"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: false, comment: "学生生日"),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "学生表");

            migrationBuilder.CreateTable(
                name: "Archives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "创建时间"),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Archives_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "档案表");

            migrationBuilder.CreateTable(
                name: "StudentCourses",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourses", x => new { x.StudentId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_StudentCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourses_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Class",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("4b501cb3-d168-4cc0-b375-48fb33f318a4"), "计算机一班" },
                    { new Guid("a715b654-c721-fe03-428a-eecc71f91fd1"), "计算机二班" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("f078ef86-080b-2d04-5cf6-2681908d4cc9"), "C#高级编程" },
                    { new Guid("a521b8b6-299a-284d-2cb6-07d8c558a244"), "Java高级编程" },
                    { new Guid("ec094517-76b3-ab8a-f09b-e5e3c8f78516"), "MySQL操作手册" },
                    { new Guid("8eee5b6d-bf73-6392-221c-14f20bb052c0"), "JavaScript入门到实践" },
                    { new Guid("1f950006-aadd-23e8-de6b-88a95d8548b2"), "C++入门编程" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "ArchiveId", "ClassId", "DateOfBirth", "Gender", "Name", "No" },
                values: new object[] { new Guid("2fa2c303-ca6e-8a08-8693-b8a7352d9c95"), new Guid("4abb3198-87fd-a81f-41a1-40dfb58f47e7"), new Guid("4b501cb3-d168-4cc0-b375-48fb33f318a4"), new DateTime(1993, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "jack", "0001" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "ArchiveId", "ClassId", "DateOfBirth", "Gender", "Name", "No" },
                values: new object[] { new Guid("590b2674-0d2d-9c2c-57f1-2813c04bdc69"), new Guid("5c496296-e9f1-80ee-1b94-5bbcbd706c49"), new Guid("a715b654-c721-fe03-428a-eecc71f91fd1"), new DateTime(1995, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "core", "0002" });

            migrationBuilder.InsertData(
                table: "Archives",
                columns: new[] { "Id", "CreateTime", "No", "StudentId" },
                values: new object[,]
                {
                    { new Guid("4abb3198-87fd-a81f-41a1-40dfb58f47e7"), new DateTime(2021, 4, 19, 10, 9, 51, 921, DateTimeKind.Local).AddTicks(1789), "A0001", new Guid("2fa2c303-ca6e-8a08-8693-b8a7352d9c95") },
                    { new Guid("5c496296-e9f1-80ee-1b94-5bbcbd706c49"), new DateTime(2021, 4, 19, 10, 9, 51, 926, DateTimeKind.Local).AddTicks(8290), "A0002", new Guid("590b2674-0d2d-9c2c-57f1-2813c04bdc69") }
                });

            migrationBuilder.InsertData(
                table: "StudentCourses",
                columns: new[] { "CourseId", "StudentId" },
                values: new object[,]
                {
                    { new Guid("f078ef86-080b-2d04-5cf6-2681908d4cc9"), new Guid("2fa2c303-ca6e-8a08-8693-b8a7352d9c95") },
                    { new Guid("a521b8b6-299a-284d-2cb6-07d8c558a244"), new Guid("2fa2c303-ca6e-8a08-8693-b8a7352d9c95") },
                    { new Guid("a521b8b6-299a-284d-2cb6-07d8c558a244"), new Guid("590b2674-0d2d-9c2c-57f1-2813c04bdc69") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Archives_StudentId",
                table: "Archives",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_CourseId",
                table: "StudentCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassId",
                table: "Students",
                column: "ClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Archives");

            migrationBuilder.DropTable(
                name: "StudentCourses");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Class");
        }
    }
}
