using Microsoft.AspNetCore.Mvc;

namespace HearMeStay.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Create(int? hotelId)
        {
            ViewData["HotelId"] = hotelId ?? 1;
            return View();
        }

        [HttpPost]
        public IActionResult Confirm()
        {
            // After booking confirmed, redirect to preference form
            return RedirectToAction("Form", "Preference");
        }
    }
}
