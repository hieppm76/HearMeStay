using Microsoft.AspNetCore.Mvc;

namespace HearMeStay.Controllers
{
    public class HotelController : Controller
    {
        public IActionResult Search(string? destination, string? checkin, string? checkout)
        {
            ViewData["Destination"] = destination ?? "Đà Nẵng";
            return View();
        }

        public IActionResult Detail(int? id)
        {
            ViewData["HotelId"] = id ?? 1;
            return View();
        }
    }
}
