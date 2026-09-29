using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerIdToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Shipments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_CustomerId",
                table: "Shipments",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Users_CustomerId",
                table: "Shipments",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Users_CustomerId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_CustomerId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Shipments");
        }
    }
}
