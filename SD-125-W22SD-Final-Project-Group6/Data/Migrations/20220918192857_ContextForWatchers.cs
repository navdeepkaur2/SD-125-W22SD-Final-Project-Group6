using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_340_W22SD_Final_Project_Group6.Data.Migrations
{
    public partial class ContextForWatchers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketWatcher_AspNetUsers_WatcherId",
                table: "TicketWatcher");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketWatcher_Tickets_TicketId",
                table: "TicketWatcher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketWatcher",
                table: "TicketWatcher");

            migrationBuilder.RenameTable(
                name: "TicketWatcher",
                newName: "TicketWatchers");

            migrationBuilder.RenameIndex(
                name: "IX_TicketWatcher_WatcherId",
                table: "TicketWatchers",
                newName: "IX_TicketWatchers_WatcherId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketWatcher_TicketId",
                table: "TicketWatchers",
                newName: "IX_TicketWatchers_TicketId");

            migrationBuilder.AlterColumn<string>(
                name: "WatcherId",
                table: "TicketWatchers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketWatchers",
                table: "TicketWatchers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketWatchers_AspNetUsers_WatcherId",
                table: "TicketWatchers",
                column: "WatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketWatchers_Tickets_TicketId",
                table: "TicketWatchers",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketWatchers_AspNetUsers_WatcherId",
                table: "TicketWatchers");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketWatchers_Tickets_TicketId",
                table: "TicketWatchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketWatchers",
                table: "TicketWatchers");

            migrationBuilder.RenameTable(
                name: "TicketWatchers",
                newName: "TicketWatcher");

            migrationBuilder.RenameIndex(
                name: "IX_TicketWatchers_WatcherId",
                table: "TicketWatcher",
                newName: "IX_TicketWatcher_WatcherId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketWatchers_TicketId",
                table: "TicketWatcher",
                newName: "IX_TicketWatcher_TicketId");

            migrationBuilder.AlterColumn<string>(
                name: "WatcherId",
                table: "TicketWatcher",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketWatcher",
                table: "TicketWatcher",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketWatcher_AspNetUsers_WatcherId",
                table: "TicketWatcher",
                column: "WatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketWatcher_Tickets_TicketId",
                table: "TicketWatcher",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
