using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Newsportal.Migrations
{
    public partial class DropTotalLikesAndRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "News");

            migrationBuilder.DropColumn(
                name: "TotalLikes",
                table: "News");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "News",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalLikes",
                table: "News",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
