using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models.Enums;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalBookingsAsync(int? accommodationId = null)
        {
            var q = _context.Bookings.AsQueryable();
            if (accommodationId.HasValue) q = q.Where(b => b.AccommodationId == accommodationId);
            return await q.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(int? accommodationId = null)
        {
            var q = _context.Bookings.Where(b => b.BookingStatus == BookingStatus.Confirmed || b.BookingStatus == BookingStatus.Completed);
            if (accommodationId.HasValue) q = q.Where(b => b.AccommodationId == accommodationId);
            return await q.SumAsync(b => b.TotalAmount);
        }

        public async Task<decimal> GetTotalCommissionAsync(int? accommodationId = null)
        {
            var q = _context.CommissionTransactions.AsQueryable();
            if (accommodationId.HasValue) q = q.Where(c => c.AccommodationId == accommodationId);
            return await q.SumAsync(c => c.CommissionAmount);
        }

        public async Task<double> GetConfirmedRateAsync(int? accommodationId = null)
        {
            var q = _context.Bookings.AsQueryable();
            if (accommodationId.HasValue) q = q.Where(b => b.AccommodationId == accommodationId);
            var total = await q.CountAsync();
            if (total == 0) return 0;
            var confirmed = await q.CountAsync(b => b.BookingStatus == BookingStatus.Confirmed || b.BookingStatus == BookingStatus.Completed);
            return (double)confirmed / total * 100;
        }

        public async Task<double> GetPreferenceFormRateAsync(int? accommodationId = null)
        {
            var q = _context.Bookings.Where(b => b.BookingStatus == BookingStatus.Confirmed || b.BookingStatus == BookingStatus.Completed);
            if (accommodationId.HasValue) q = q.Where(b => b.AccommodationId == accommodationId);
            var total = await q.CountAsync();
            if (total == 0) return 0;
            var withPref = await q.CountAsync(b => b.GuestPreference != null);
            return (double)withPref / total * 100;
        }

        public async Task<double> GetAverageRatingAsync(int? accommodationId = null)
        {
            var q = _context.Reviews.Where(r => r.IsVisible);
            if (accommodationId.HasValue) q = q.Where(r => r.AccommodationId == accommodationId);
            if (!await q.AnyAsync()) return 0;
            return await q.AverageAsync(r => r.Rating);
        }

        public async Task<double> GetAveragePersonalizationRatingAsync(int? accommodationId = null)
        {
            var q = _context.Reviews.Where(r => r.IsVisible);
            if (accommodationId.HasValue) q = q.Where(r => r.AccommodationId == accommodationId);
            if (!await q.AnyAsync()) return 0;
            return await q.AverageAsync(r => r.PersonalizationRating);
        }

        public async Task<Dictionary<string, int>> GetCommonTagsAsync(int? accommodationId = null)
        {
            var q = _context.GuestInsightTags.AsQueryable();
            if (accommodationId.HasValue)
            {
                q = q.Where(t => t.GuestInsight.GuestPreference.Booking.AccommodationId == accommodationId);
            }
            return await q.GroupBy(t => t.TagName)
                .Select(g => new { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToDictionaryAsync(x => x.Tag, x => x.Count);
        }
    }
}
