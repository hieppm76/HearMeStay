using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.ViewModels;

namespace HearMeStay.Controllers
{
    [Authorize(Roles = "Traveler")]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int bookingId)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Accommodation).Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) return NotFound();
            if (booking.BookingStatus != BookingStatus.Completed)
            {
                TempData["Error"] = "Chỉ có thể đánh giá khi lưu trú đã hoàn tất.";
                return RedirectToAction("Details", "Bookings", new { id = bookingId });
            }
            if (booking.Review != null)
            {
                TempData["Error"] = "Bạn đã đánh giá đặt phòng này rồi.";
                return RedirectToAction("Details", "Bookings", new { id = bookingId });
            }

            return View(new ReviewCreateViewModel
            {
                BookingId = booking.Id,
                BookingCode = booking.BookingCode,
                AccommodationName = booking.Accommodation.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings.Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.Id == model.BookingId && b.UserId == userId);

            if (booking == null || booking.BookingStatus != BookingStatus.Completed || booking.Review != null)
                return NotFound();

            var review = new Review
            {
                BookingId = model.BookingId,
                UserId = userId!,
                AccommodationId = booking.AccommodationId,
                Rating = model.Rating,
                PersonalizationRating = model.PersonalizationRating,
                DidHotelPrepareWell = model.DidHotelPrepareWell,
                Comment = model.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cảm ơn bạn đã chia sẻ đánh giá!";
            return RedirectToAction("MyBookings", "Bookings");
        }
    }
}
