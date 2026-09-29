using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCourierIdToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourierId",
                table: "Shipments",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourierId",
                table: "Shipments");
        }
    }
}
