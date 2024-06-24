using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neru.Migrations
{
    /// <inheritdoc />
    public partial class InitializeSqliteDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRemembrances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    UserNickname = table.Column<string>(type: "TEXT", nullable: false),
                    UserFavouriteWord = table.Column<string>(type: "TEXT", nullable: false),
                    UserHP = table.Column<int>(type: "INTEGER", nullable: false),
                    UserLove = table.Column<float>(type: "REAL", nullable: false),
                    UserBirthday = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRemembrances", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRemembrances");
        }
    }
}
