using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public BookingService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            booking.BookingCode = GenerateBookingCode();
            booking.BookingStatus = BookingStatus.Pending;
            booking.PaymentMethod = PaymentMethod.PayAtHotel;
            booking.PaymentStatus = PaymentStatus.Unpaid;
            booking.CreatedAt = DateTime.Now;

            var nights = (booking.CheckOutDate - booking.CheckInDate).Days;
            var roomType = await _context.RoomTypes.FindAsync(booking.RoomTypeId);
            if (roomType != null)
            {
                booking.TotalAmount = CalculateTotalAmount(roomType.PricePerNight, booking.CheckInDate, booking.CheckOutDate, booking.NumberOfRooms);
                booking.CommissionAmount = CalculateCommission(booking.TotalAmount, booking.CommissionRate);
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Notify hotel partner
            var accommodation = await _context.Accommodations.FindAsync(booking.AccommodationId);
            if (accommodation != null)
            {
                await _notificationService.CreateNotificationAsync(
                    accommodation.OwnerId,
                    "Đặt phòng mới",
                    $"Bạn có đặt phòng mới #{booking.BookingCode} cần xác nhận.",
                    "BookingCreated");
            }

            return booking;
        }

        public async Task<Booking?> ConfirmBookingAsync(int bookingId, string? partnerNote = null)
        {
            var booking = await _context.Bookings.Include(b => b.Accommodation).FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null || booking.BookingStatus != BookingStatus.Pending) return null;

            booking.BookingStatus = BookingStatus.Confirmed;
            booking.ConfirmedAt = DateTime.Now;
            booking.PartnerResponseNote = partnerNote;

            // Create commission transaction
            var commission = new CommissionTransaction
            {
                BookingId = booking.Id,
                AccommodationId = booking.AccommodationId,
                TotalAmount = booking.TotalAmount,
                CommissionRate = booking.CommissionRate,
                CommissionAmount = booking.CommissionAmount,
                Status = CommissionStatus.Payable
            };
            _context.CommissionTransactions.Add(commission);
            await _context.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                booking.UserId,
                "Đặt phòng đã được xác nhận",
                $"Đặt phòng #{booking.BookingCode} đã được xác nhận. Bạn có thể điền form nhu cầu cá nhân.",
                "BookingConfirmed");

            return booking;
        }

        public async Task<Booking?> RejectBookingAsync(int bookingId, string? partnerNote = null)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null || booking.BookingStatus != BookingStatus.Pending) return null;

            booking.BookingStatus = BookingStatus.Rejected;
            booking.PartnerResponseNote = partnerNote;
            await _context.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                booking.UserId,
                "Đặt phòng bị từ chối",
                $"Đặt phòng #{booking.BookingCode} đã bị từ chối. Lý do: {partnerNote ?? "Không có lý do cụ thể."}",
                "BookingRejected");

            return booking;
        }

        public async Task<Booking?> CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return null;
            if (booking.BookingStatus != BookingStatus.Pending && booking.BookingStatus != BookingStatus.Confirmed) return null;

            booking.BookingStatus = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking?> MarkCompletedAsync(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null || booking.BookingStatus != BookingStatus.Confirmed) return null;

            booking.BookingStatus = BookingStatus.Completed;
            await _context.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                booking.UserId,
                "Lưu trú hoàn tất",
                $"Cảm ơn bạn đã lưu trú! Hãy chia sẻ đánh giá cho đặt phòng #{booking.BookingCode}.",
                "BookingCompleted");

            return booking;
        }

        public decimal CalculateTotalAmount(decimal pricePerNight, DateTime checkIn, DateTime checkOut, int numberOfRooms)
        {
            var nights = (checkOut - checkIn).Days;
            if (nights <= 0) nights = 1;
            return pricePerNight * nights * numberOfRooms;
        }

        public decimal CalculateCommission(decimal totalAmount, decimal commissionRate = 0.08m)
        {
            return totalAmount * commissionRate;
        }

        public string GenerateBookingCode()
        {
            return $"HMS{DateTime.Now:yyyyMMdd}{Random.Shared.Next(10000, 99999)}";
        }
    }
}
