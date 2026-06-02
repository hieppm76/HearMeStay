using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;

namespace HearMeStay.Areas.Partner.Controllers
{
    [Area("Partner")]
    [Authorize(Roles = "HotelPartner")]
    public class GuestInsightsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GuestInsightsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var insights = await _context.GuestInsights
                .Where(gi => gi.GuestPreference.Booking.Accommodation.OwnerId == userId)
                .Include(gi => gi.GuestPreference).ThenInclude(p => p.Booking).ThenInclude(b => b.User)
                .Include(gi => gi.Tags)
                .Include(gi => gi.Tasks)
                .OrderByDescending(gi => gi.CreatedAt)
                .ToListAsync();
            return View(insights);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var insight = await _context.GuestInsights
                .Include(gi => gi.GuestPreference).ThenInclude(p => p.Booking).ThenInclude(b => b.Accommodation)
                .Include(gi => gi.GuestPreference).ThenInclude(p => p.Booking).ThenInclude(b => b.User)
                .Include(gi => gi.GuestPreference).ThenInclude(p => p.Booking).ThenInclude(b => b.RoomType)
                .Include(gi => gi.Tags)
                .Include(gi => gi.Tasks)
                .Include(gi => gi.UpsellSuggestions)
                .FirstOrDefaultAsync(gi => gi.Id == id && gi.GuestPreference.Booking.Accommodation.OwnerId == userId);

            if (insight == null) return NotFound();
            return View(insight);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, GuestTaskStatus status, string? partnerNote)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _context.GuestTasks
                .Include(t => t.GuestInsight).ThenInclude(gi => gi.GuestPreference).ThenInclude(p => p.Booking).ThenInclude(b => b.Accommodation)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.GuestInsight.GuestPreference.Booking.Accommodation.OwnerId == userId);

            if (task == null) return NotFound();

            task.TaskStatus = status;
            task.PartnerNote = partnerNote;
            task.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật trạng thái công việc.";
            return RedirectToAction("Details", new { id = task.GuestInsightId });
        }
    }
}
