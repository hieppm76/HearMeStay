using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;

namespace HearMeStay.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BookingsController(ApplicationDbContext ctx) { _context = ctx; }
        public async Task<IActionResult> Index() => View(await _context.Bookings.Include(b => b.User).Include(b => b.Accommodation).Include(b => b.RoomType).OrderByDescending(b => b.CreatedAt).ToListAsync());
        public async Task<IActionResult> Details(int id) => View(await _context.Bookings.Include(b => b.User).Include(b => b.Accommodation).Include(b => b.RoomType).Include(b => b.GuestPreference).FirstOrDefaultAsync(b => b.Id == id));
    }
}
