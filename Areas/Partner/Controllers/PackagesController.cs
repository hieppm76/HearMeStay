using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HearMeStay.Areas.Partner.Controllers
{
    [Area("Partner")]
    [Authorize(Roles = "HotelPartner")]
    public class PackagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
