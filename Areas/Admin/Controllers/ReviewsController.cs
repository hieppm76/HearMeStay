using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;

namespace HearMeStay.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReviewsController(ApplicationDbContext ctx) { _context = ctx; }
        public async Task<IActionResult> Index() => View(await _context.Reviews.Include(r => r.User).Include(r => r.Accommodation).OrderByDescending(r => r.CreatedAt).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Hide(int id) { var r = await _context.Reviews.FindAsync(id); if (r != null) { r.IsVisible = false; await _context.SaveChangesAsync(); } TempData["Success"] = "Đã ẩn đánh giá."; return RedirectToAction("Index"); }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Show(int id) { var r = await _context.Reviews.FindAsync(id); if (r != null) { r.IsVisible = true; await _context.SaveChangesAsync(); } TempData["Success"] = "Đã hiển thị đánh giá."; return RedirectToAction("Index"); }
    }
}
