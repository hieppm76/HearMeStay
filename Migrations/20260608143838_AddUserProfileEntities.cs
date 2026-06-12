using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HearMeStay.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPreferenceProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FoodPreferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllergyNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomPreferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServicePreferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityInterests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HealthNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsentToStoreHealthNotes = table.Column<bool>(type: "bit", nullable: false),
                    ConsentToShareWithHotel = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferenceProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferenceProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferenceTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserPreferenceProfileId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReusable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedFromBookingId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferenceTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferenceTags_UserPreferenceProfiles_UserPreferenceProfileId",
                        column: x => x.UserPreferenceProfileId,
                        principalTable: "UserPreferenceProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferenceProfiles_UserId",
                table: "UserPreferenceProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferenceTags_UserPreferenceProfileId",
                table: "UserPreferenceTags",
                column: "UserPreferenceProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferenceTags");

            migrationBuilder.DropTable(
                name: "UserPreferenceProfiles");
        }
    }
}
