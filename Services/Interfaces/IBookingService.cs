using HearMeStay.Models;

namespace HearMeStay.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking?> ConfirmBookingAsync(int bookingId, string? partnerNote = null);
        Task<Booking?> SubmitPaymentProofAsync(int bookingId, string transferContent, string? proofImageUrl = null);
        Task<Booking?> VerifyPaymentAsync(int bookingId, string adminOrPartnerName);
        Task<Booking?> RejectBookingAsync(int bookingId, string? partnerNote = null);
        Task<Booking?> CancelBookingAsync(int bookingId);
        Task<Booking?> MarkCompletedAsync(int bookingId);
        decimal CalculateTotalAmount(decimal pricePerNight, DateTime checkIn, DateTime checkOut, int numberOfRooms);
        decimal CalculateCommission(decimal totalAmount, decimal commissionRate = 0.08m);
        string GenerateBookingCode();
    }
}
