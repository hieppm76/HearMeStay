using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models.Enums;

namespace HearMeStay.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize(Roles = "Admin")]
    public class CommissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CommissionsController(ApplicationDbContext ctx) { _context = ctx; }
        public async Task<IActionResult> Index() => View(await _context.CommissionTransactions.Include(c => c.Booking).Include(c => c.Accommodation).OrderByDescending(c => c.CreatedAt).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var c = await _context.CommissionTransactions.FindAsync(id);
            if (c != null) { c.Status = CommissionStatus.Paid; c.PaidAt = DateTime.Now; await _context.SaveChangesAsync(); }
            TempData["Success"] = "Đã đánh dấu thanh toán."; return RedirectToAction("Index");
        }
    }
}
