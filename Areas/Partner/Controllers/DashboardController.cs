using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Areas.Partner.Controllers
{
    [Area("Partner")]
    [Authorize(Roles = "HotelPartner")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReportService _reportService;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IReportService reportService)
        {
            _context = context;
            _userManager = userManager;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var accommodation = await _context.Accommodations.FirstOrDefaultAsync(a => a.OwnerId == userId);
            if (accommodation == null)
            {
                ViewBag.NoAccommodation = true;
                return View();
            }

            var accId = accommodation.Id;
            ViewBag.AccommodationName = accommodation.Name;
            ViewBag.AccommodationStatus = accommodation.Status;
            ViewBag.PendingBookings = await _context.Bookings.CountAsync(b => b.AccommodationId == accId && b.BookingStatus == BookingStatus.Pending);
            ViewBag.ConfirmedBookings = await _context.Bookings.CountAsync(b => b.AccommodationId == accId && b.BookingStatus == BookingStatus.Confirmed);
            ViewBag.UpcomingCheckIns = await _context.Bookings.CountAsync(b => b.AccommodationId == accId && b.BookingStatus == BookingStatus.Confirmed && b.CheckInDate <= DateTime.Now.AddDays(7) && b.CheckInDate >= DateTime.Now);
            ViewBag.HighPriorityInsights = await _context.GuestInsights.CountAsync(gi => gi.GuestPreference.Booking.AccommodationId == accId && (gi.PriorityLevel == PriorityLevel.High || gi.PriorityLevel == PriorityLevel.Critical));
            ViewBag.Revenue = await _reportService.GetTotalRevenueAsync(accId);
            ViewBag.AvgRating = await _reportService.GetAverageRatingAsync(accId);
            ViewBag.PreferenceRate = await _reportService.GetPreferenceFormRateAsync(accId);

            return View();
        }
    }
}
