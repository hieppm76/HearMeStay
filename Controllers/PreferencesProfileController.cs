using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;

namespace HearMeStay.Controllers
{
    [Authorize(Roles = "Traveler")]
    public class PreferencesProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PreferencesProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyProfile()
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _context.UserPreferenceProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserPreferenceProfile model)
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _context.UserPreferenceProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                model.UserId = userId;
                _context.UserPreferenceProfiles.Add(model);
            }
            else
            {
                profile.FoodPreferences = model.FoodPreferences;
                profile.AllergyNotes = model.AllergyNotes;
                profile.RoomPreferences = model.RoomPreferences;
                profile.ServicePreferences = model.ServicePreferences;
                profile.ActivityInterests = model.ActivityInterests;
                profile.HealthNotes = model.HealthNotes;
                profile.ConsentToStoreHealthNotes = model.ConsentToStoreHealthNotes;
                profile.IsActive = model.IsActive;
                profile.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Hồ sơ sở thích đã được cập nhật thành công.";
            
            return RedirectToAction(nameof(MyProfile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _context.UserPreferenceProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile != null)
            {
                _context.UserPreferenceProfiles.Remove(profile);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hồ sơ sở thích đã được xóa hoàn toàn khỏi hệ thống.";
            }

            return RedirectToAction(nameof(MyProfile));
        }
    }
}
