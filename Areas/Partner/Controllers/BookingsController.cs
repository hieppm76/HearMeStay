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
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingService _bookingService;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBookingService bookingService)
        {
            _context = context;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _context.Bookings
                .Where(b => b.Accommodation.OwnerId == userId)
                .Include(b => b.Accommodation).Include(b => b.RoomType).Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt).ToListAsync();
            return View(bookings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Accommodation).Include(b => b.RoomType).Include(b => b.User)
                .Include(b => b.GuestPreference).ThenInclude(p => p!.GuestInsight)
                .FirstOrDefaultAsync(b => b.Id == id && b.Accommodation.OwnerId == userId);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id, string? partnerNote)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings.Include(b => b.Accommodation).FirstOrDefaultAsync(b => b.Id == id && b.Accommodation.OwnerId == userId);
            if (booking == null) return NotFound();
            await _bookingService.ConfirmBookingAsync(id, partnerNote);
            TempData["Success"] = "Booking đã được xác nhận. Khách có thể điền form nhu cầu cá nhân.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? partnerNote)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings.Include(b => b.Accommodation).FirstOrDefaultAsync(b => b.Id == id && b.Accommodation.OwnerId == userId);
            if (booking == null) return NotFound();
            await _bookingService.RejectBookingAsync(id, partnerNote);
            TempData["Success"] = "Đã từ chối đặt phòng.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings.Include(b => b.Accommodation).FirstOrDefaultAsync(b => b.Id == id && b.Accommodation.OwnerId == userId);
            if (booking == null) return NotFound();
            await _bookingService.MarkCompletedAsync(id);
            TempData["Success"] = "Lưu trú đã hoàn tất.";
            return RedirectToAction("Index");
        }
    }
}
