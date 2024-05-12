using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wppReservations.Migrations
{
    /// <inheritdoc />
    public partial class calendarId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendarId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendarId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Reservations");
        }
    }
}
