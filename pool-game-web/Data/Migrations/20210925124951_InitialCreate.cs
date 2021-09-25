using Microsoft.EntityFrameworkCore.Migrations;

namespace pool_game_web.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoolTables",
                columns: table => new
                {
                    PoolTableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolTables", x => x.PoolTableId);
                });

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    VisitorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.VisitorId);
                });

            migrationBuilder.CreateTable(
                name: "Reservationss",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitorId = table.Column<int>(type: "int", nullable: false),
                    PoolTableId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservationss", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservationss_PoolTables_PoolTableId",
                        column: x => x.PoolTableId,
                        principalTable: "PoolTables",
                        principalColumn: "PoolTableId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservationss_Visitors_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitors",
                        principalColumn: "VisitorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservationss_PoolTableId",
                table: "Reservationss",
                column: "PoolTableId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservationss_VisitorId",
                table: "Reservationss",
                column: "VisitorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservationss");

            migrationBuilder.DropTable(
                name: "PoolTables");

            migrationBuilder.DropTable(
                name: "Visitors");
        }
    }
}
