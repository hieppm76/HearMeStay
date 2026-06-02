using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReportService _reportService;
        public DashboardController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, IReportService rs) { _context = ctx; _userManager = um; _reportService = rs; }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalPartners = (await _userManager.GetUsersInRoleAsync("HotelPartner")).Count;
            ViewBag.TotalAccommodations = await _context.Accommodations.CountAsync();
            ViewBag.PendingAccommodations = await _context.Accommodations.CountAsync(a => a.Status == AccommodationStatus.Pending);
            ViewBag.TotalBookings = await _reportService.GetTotalBookingsAsync();
            ViewBag.TotalCommission = await _reportService.GetTotalCommissionAsync();
            ViewBag.AvgRating = await _reportService.GetAverageRatingAsync();
            ViewBag.RecentBookings = await _context.Bookings.Include(b => b.User).Include(b => b.Accommodation).OrderByDescending(b => b.CreatedAt).Take(5).ToListAsync();
            return View();
        }
    }
}
