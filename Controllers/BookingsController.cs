using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.ViewModels;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Controllers
{
    [Authorize(Roles = "Traveler")]
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

        [HttpGet]
        public async Task<IActionResult> Create(int roomTypeId)
        {
            var roomType = await _context.RoomTypes.Include(r => r.Accommodation).FirstOrDefaultAsync(r => r.Id == roomTypeId);
            if (roomType == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var model = new BookingCreateViewModel
            {
                AccommodationId = roomType.AccommodationId,
                AccommodationName = roomType.Accommodation.Name,
                RoomTypeId = roomType.Id,
                RoomTypeName = roomType.Name,
                PricePerNight = roomType.PricePerNight,
                GuestFullName = user?.FullName ?? "",
                GuestEmail = user?.Email ?? "",
                CheckInDate = DateTime.Today.AddDays(1),
                CheckOutDate = DateTime.Today.AddDays(2)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (model.CheckOutDate <= model.CheckInDate)
                ModelState.AddModelError("CheckOutDate", "Ngày trả phòng phải sau ngày nhận phòng.");

            if (!ModelState.IsValid)
            {
                var rt = await _context.RoomTypes.Include(r => r.Accommodation).FirstOrDefaultAsync(r => r.Id == model.RoomTypeId);
                if (rt != null) { model.AccommodationName = rt.Accommodation.Name; model.RoomTypeName = rt.Name; model.PricePerNight = rt.PricePerNight; }
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var booking = new Booking
            {
                UserId = user!.Id,
                AccommodationId = model.AccommodationId,
                RoomTypeId = model.RoomTypeId,
                GuestFullName = model.GuestFullName,
                GuestEmail = model.GuestEmail,
                GuestPhone = model.GuestPhone,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfGuests = model.NumberOfGuests,
                NumberOfRooms = model.NumberOfRooms,
                GuestNote = model.GuestNote
            };

            await _bookingService.CreateBookingAsync(booking);
            TempData["Success"] = "Đặt phòng thành công! Đang chờ khách sạn xác nhận.";
            return RedirectToAction("MyBookings");
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Accommodation)
                .Include(b => b.RoomType)
                .Include(b => b.GuestPreference)
                .Include(b => b.Review)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return View(bookings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Accommodation)
                .Include(b => b.RoomType)
                .Include(b => b.GuestPreference).ThenInclude(p => p!.GuestInsight)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (booking == null) return NotFound();

            var result = await _bookingService.CancelBookingAsync(id);
            if (result == null)
            {
                TempData["Error"] = "Không thể hủy đặt phòng này.";
                return RedirectToAction("Details", new { id });
            }

            TempData["Success"] = "Đã hủy đặt phòng thành công.";
            return RedirectToAction("MyBookings");
        }
    }
}
