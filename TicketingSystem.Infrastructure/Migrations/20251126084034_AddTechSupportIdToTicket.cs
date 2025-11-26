using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechSupportIdToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_TicketMessages_TicketMessageId",
                table: "Attachments");

            migrationBuilder.AddColumn<int>(
                name: "TechSupportId",
                table: "Tickets",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TicketMessageId",
                table: "Attachments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TechSupportId",
                table: "Tickets",
                column: "TechSupportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_TicketMessages_TicketMessageId",
                table: "Attachments",
                column: "TicketMessageId",
                principalTable: "TicketMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_TechSupportId",
                table: "Tickets",
                column: "TechSupportId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_TicketMessages_TicketMessageId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_TechSupportId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TechSupportId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TechSupportId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "TicketMessageId",
                table: "Attachments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_TicketMessages_TicketMessageId",
                table: "Attachments",
                column: "TicketMessageId",
                principalTable: "TicketMessages",
                principalColumn: "Id");
        }
    }
}
