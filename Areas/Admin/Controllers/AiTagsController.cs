using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;

namespace HearMeStay.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize(Roles = "Admin")]
    public class AiTagsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AiTagsController(ApplicationDbContext ctx) { _context = ctx; }
        public async Task<IActionResult> Index() => View(await _context.GuestInsightTags.Include(t => t.GuestInsight).ThenInclude(gi => gi.GuestPreference).ThenInclude(p => p.Booking).OrderByDescending(t => t.Id).Take(100).ToListAsync());
    }
}
