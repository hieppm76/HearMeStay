using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HearMeStay.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDeadline",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProofImageUrl",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentQrImageUrl",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTransferContent",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentVerifiedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentVerifiedBy",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDeadline",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentProofImageUrl",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentQrImageUrl",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentTransferContent",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentVerifiedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentVerifiedBy",
                table: "Bookings");
        }
    }
}
