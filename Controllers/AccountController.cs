using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HearMeStay.Models;
using HearMeStay.ViewModels;

namespace HearMeStay.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    if (roles.Contains("HotelPartner"))
                        return RedirectToAction("Index", "Dashboard", new { area = "Partner" });
                }
                return LocalRedirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // All public registrations are Traveler only
                await _userManager.AddToRoleAsync(user, "Traveler");
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với HearMeStay.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                var msg = error.Code switch
                {
                    "DuplicateEmail" or "DuplicateUserName" => "Email này đã được sử dụng.",
                    "PasswordTooShort" => "Mật khẩu quá ngắn.",
                    "PasswordRequiresUpper" => "Mật khẩu cần có chữ hoa.",
                    "PasswordRequiresLower" => "Mật khẩu cần có chữ thường.",
                    "PasswordRequiresDigit" => "Mật khẩu cần có chữ số.",
                    "PasswordRequiresNonAlphanumeric" => "Mật khẩu cần có ký tự đặc biệt.",
                    _ => error.Description
                };
                ModelState.AddModelError(string.Empty, msg);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
