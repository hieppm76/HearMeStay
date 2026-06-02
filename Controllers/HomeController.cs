using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models.Enums;
using HearMeStay.ViewModels;

namespace HearMeStay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Featured accommodations (approved, active)
            var featured = await _context.Accommodations
                .Where(a => a.Status == AccommodationStatus.Approved && a.IsActive)
                .Include(a => a.Images)
                .Include(a => a.RoomTypes)
                .Include(a => a.Reviews)
                .Take(6)
                .Select(a => new AccommodationCardViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    City = a.City,
                    Province = a.Province,
                    AccommodationType = a.AccommodationType,
                    StarRating = a.StarRating,
                    MainImageUrl = a.Images.Where(i => i.IsMain).Select(i => i.ImageUrl).FirstOrDefault() ?? "/images/placeholder-hotel.svg",
                    MinPrice = a.RoomTypes.Any() ? a.RoomTypes.Min(r => r.PricePerNight) : 0,
                    AverageRating = a.Reviews.Any(r => r.IsVisible) ? a.Reviews.Where(r => r.IsVisible).Average(r => r.Rating) : 0,
                    ReviewCount = a.Reviews.Count(r => r.IsVisible),
                    HasQuietRoom = a.RoomTypes.Any(r => r.IsQuietRoom),
                    HasVeganMeal = a.RoomTypes.Any(r => r.SupportsVeganMeal),
                    HasAllergySupport = a.RoomTypes.Any(r => r.SupportsAllergyRequest)
                })
                .ToListAsync();

            return View(featured);
        }

        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult BecomePartner() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new Models.ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
