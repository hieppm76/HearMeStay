using Microsoft.AspNetCore.Mvc;

namespace HearMeStay.Controllers
{
    public class PreferenceController : Controller
    {
        public IActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit()
        {
            // Process AI preference form submission
            TempData["Success"] = "Đã gửi nhu cầu thành công!";
            return RedirectToAction("Form");
        }
    }
}
