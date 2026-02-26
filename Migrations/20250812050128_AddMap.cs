using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomFInder.Migrations
{
    public partial class AddMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Rooms",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Rooms",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Rooms");
        }
    }
}
