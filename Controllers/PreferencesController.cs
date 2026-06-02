using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.ViewModels;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Controllers
{
    [Authorize(Roles = "Traveler")]
    public class PreferencesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPreferenceAnalysisService _analysisService;
        private readonly INotificationService _notificationService;

        public PreferencesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IPreferenceAnalysisService analysisService, INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _analysisService = analysisService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int bookingId)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Accommodation)
                .Include(b => b.GuestPreference)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) return NotFound();
            if (booking.BookingStatus != BookingStatus.Confirmed)
            {
                TempData["Error"] = "Chỉ có thể điền form khi đặt phòng đã được xác nhận.";
                return RedirectToAction("Details", "Bookings", new { id = bookingId });
            }
            if (booking.GuestPreference != null)
                return RedirectToAction("Details", new { bookingId });

            var model = new GuestPreferenceCreateViewModel
            {
                BookingId = booking.Id,
                BookingCode = booking.BookingCode,
                AccommodationName = booking.Accommodation.Name
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GuestPreferenceCreateViewModel model)
        {
            if (!model.ConsentToShareWithHotel)
            {
                ModelState.AddModelError("ConsentToShareWithHotel", "Vui lòng đồng ý chia sẻ thông tin với nơi lưu trú để tiếp tục.");
                var b = await _context.Bookings.Include(x => x.Accommodation).FirstOrDefaultAsync(x => x.Id == model.BookingId);
                if (b != null) { model.BookingCode = b.BookingCode; model.AccommodationName = b.Accommodation.Name; }
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings.Include(x => x.Accommodation).FirstOrDefaultAsync(b => b.Id == model.BookingId && b.UserId == userId);
            if (booking == null || booking.BookingStatus != BookingStatus.Confirmed) return NotFound();

            var pref = new GuestPreference
            {
                BookingId = model.BookingId,
                RawText = model.RawText,
                HasFoodAllergy = model.HasFoodAllergy,
                FoodAllergyDetail = model.FoodAllergyDetail,
                DietPreference = model.DietPreference,
                RoomPreference = model.RoomPreference,
                HealthNote = model.HealthNote,
                SpecialOccasion = model.SpecialOccasion,
                TravelPurpose = model.TravelPurpose,
                ActivityInterest = model.ActivityInterest,
                NeedAirportPickup = model.NeedAirportPickup,
                NeedEarlyCheckIn = model.NeedEarlyCheckIn,
                NeedDecoration = model.NeedDecoration,
                ConsentToShareWithHotel = model.ConsentToShareWithHotel
            };

            _context.GuestPreferences.Add(pref);
            await _context.SaveChangesAsync();

            // Run AI analysis
            await _analysisService.AnalyzePreferenceAsync(pref);

            // Notify partner
            await _notificationService.CreateNotificationAsync(
                booking.Accommodation.OwnerId,
                "Khách đã chia sẻ nhu cầu cá nhân",
                $"Khách đặt phòng #{booking.BookingCode} đã điền form nhu cầu. Xem Guest Insight để chuẩn bị.",
                "PreferenceSubmitted");

            TempData["Success"] = "Thông tin của bạn đã được gửi. Nơi lưu trú sẽ chuẩn bị dựa trên nhu cầu phù hợp.";
            return RedirectToAction("Details", new { bookingId = model.BookingId });
        }

        public async Task<IActionResult> Details(int bookingId)
        {
            var userId = _userManager.GetUserId(User);
            var pref = await _context.GuestPreferences
                .Include(p => p.Booking).ThenInclude(b => b.Accommodation)
                .Include(p => p.GuestInsight).ThenInclude(gi => gi!.Tags)
                .FirstOrDefaultAsync(p => p.Booking.Id == bookingId && p.Booking.UserId == userId);

            if (pref == null) return NotFound();
            return View(pref);
        }
    }
}
