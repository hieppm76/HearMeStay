using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Areas.Partner.Controllers
{
    [Area("Partner")][Authorize(Roles = "HotelPartner")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReportService _reportService;
        public ReportsController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, IReportService rs) { _context = ctx; _userManager = um; _reportService = rs; }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var acc = await _context.Accommodations.FirstOrDefaultAsync(a => a.OwnerId == userId);
            if (acc == null) return View();
            var accId = acc.Id;
            ViewBag.TotalBookings = await _reportService.GetTotalBookingsAsync(accId);
            ViewBag.Revenue = await _reportService.GetTotalRevenueAsync(accId);
            ViewBag.Commission = await _reportService.GetTotalCommissionAsync(accId);
            ViewBag.ConfirmedRate = await _reportService.GetConfirmedRateAsync(accId);
            ViewBag.PreferenceRate = await _reportService.GetPreferenceFormRateAsync(accId);
            ViewBag.AvgRating = await _reportService.GetAverageRatingAsync(accId);
            ViewBag.AvgPersonalization = await _reportService.GetAveragePersonalizationRatingAsync(accId);
            ViewBag.CommonTags = await _reportService.GetCommonTagsAsync(accId);
            return View();
        }
    }
}
