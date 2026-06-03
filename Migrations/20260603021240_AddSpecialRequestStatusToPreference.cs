using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HearMeStay.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialRequestStatusToPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartnerSpecialRequestNote",
                table: "GuestPreferences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecialRequestStatus",
                table: "GuestPreferences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartnerSpecialRequestNote",
                table: "GuestPreferences");

            migrationBuilder.DropColumn(
                name: "SpecialRequestStatus",
                table: "GuestPreferences");
        }
    }
}
