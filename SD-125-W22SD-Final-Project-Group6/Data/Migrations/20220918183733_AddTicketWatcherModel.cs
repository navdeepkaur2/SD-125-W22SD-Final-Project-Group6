using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_340_W22SD_Final_Project_Group6.Data.Migrations
{
    public partial class AddTicketWatcherModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Tickets",
                newName: "ApplicationUser");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_OwnerId",
                table: "Tickets",
                newName: "IX_Tickets_ApplicationUser");

            migrationBuilder.CreateTable(
                name: "TicketWatcher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WatcherId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketWatcher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketWatcher_AspNetUsers_WatcherId",
                        column: x => x.WatcherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketWatcher_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketWatcher_TicketId",
                table: "TicketWatcher",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketWatcher_WatcherId",
                table: "TicketWatcher",
                column: "WatcherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ApplicationUser",
                table: "Tickets",
                column: "ApplicationUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ApplicationUser",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketWatcher");

            migrationBuilder.RenameColumn(
                name: "ApplicationUser",
                table: "Tickets",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ApplicationUser",
                table: "Tickets",
                newName: "IX_Tickets_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
