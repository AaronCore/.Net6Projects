using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VueAdmin.EntityFrameworkCore.DbMigrations.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VA_Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Account = table.Column<string>(maxLength: 200, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: true),
                    RoleId = table.Column<int>(nullable: false),
                    Sort = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    EditTime = table.Column<DateTime>(nullable: true),
                    Editor = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Creater = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VA_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VA_Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Sort = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    EditTime = table.Column<DateTime>(nullable: true),
                    Editor = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Creater = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VA_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VA_RoleMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VA_RoleMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VA_Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Sort = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    EditTime = table.Column<DateTime>(nullable: true),
                    Editor = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Creater = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VA_Roles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VA_Accounts");

            migrationBuilder.DropTable(
                name: "VA_Menus");

            migrationBuilder.DropTable(
                name: "VA_RoleMenus");

            migrationBuilder.DropTable(
                name: "VA_Roles");
        }
    }
}
