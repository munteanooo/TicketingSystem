using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Domain_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMessages_Tickets_TicketId1",
                table: "TicketMessages");

            migrationBuilder.DropIndex(
                name: "IX_TicketMessages_TicketId1",
                table: "TicketMessages");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "TicketMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketId1",
                table: "TicketMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_TicketId1",
                table: "TicketMessages",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMessages_Tickets_TicketId1",
                table: "TicketMessages",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
