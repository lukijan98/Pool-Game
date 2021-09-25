using Microsoft.EntityFrameworkCore.Migrations;

namespace pool_game_web.Data.Migrations
{
    public partial class tester2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VisitorName",
                table: "Visitors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReservationName",
                table: "Reservationss",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PoolTableNumber",
                table: "PoolTables",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitorName",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "ReservationName",
                table: "Reservationss");

            migrationBuilder.DropColumn(
                name: "PoolTableNumber",
                table: "PoolTables");
        }
    }
}
