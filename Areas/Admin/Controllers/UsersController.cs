using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Models;

namespace HearMeStay.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(UserManager<ApplicationUser> um) { _userManager = um; }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
            return View(users);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null) { user.IsActive = false; await _userManager.UpdateAsync(user); await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); }
            TempData["Success"] = "Đã khóa tài khoản."; return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null) { user.IsActive = true; await _userManager.UpdateAsync(user); await _userManager.SetLockoutEndDateAsync(user, null); }
            TempData["Success"] = "Đã mở khóa tài khoản."; return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToPartner(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Contains("Traveler"))
                    await _userManager.RemoveFromRoleAsync(user, "Traveler");
                if (!currentRoles.Contains("HotelPartner"))
                    await _userManager.AddToRoleAsync(user, "HotelPartner");
                TempData["Success"] = $"Đã nâng cấp {user.FullName} thành Đối tác.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DemoteToTraveler(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Contains("HotelPartner"))
                    await _userManager.RemoveFromRoleAsync(user, "HotelPartner");
                if (!currentRoles.Contains("Traveler"))
                    await _userManager.AddToRoleAsync(user, "Traveler");
                TempData["Success"] = $"Đã chuyển {user.FullName} về Khách du lịch.";
            }
            return RedirectToAction("Index");
        }
    }
}
