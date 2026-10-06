using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDelayNotifiedAtToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DelayNotifiedAt",
                table: "Shipments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelayNotifiedAt",
                table: "Shipments");
        }
    }
}
