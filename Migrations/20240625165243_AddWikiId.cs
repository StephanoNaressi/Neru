using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neru.Migrations
{
    /// <inheritdoc />
    public partial class AddWikiId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntryID",
                table: "MyWikis",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryID",
                table: "MyWikis");
        }
    }
}
