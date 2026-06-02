using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Areas.Partner.Controllers
{
    [Area("Partner")]
    [Authorize(Roles = "HotelPartner")]
    public class AccommodationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public AccommodationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFileUploadService fileUploadService)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> MyAccommodation()
        {
            var userId = _userManager.GetUserId(User);
            var acc = await _context.Accommodations
                .Include(a => a.Images).Include(a => a.RoomTypes)
                .FirstOrDefaultAsync(a => a.OwnerId == userId);
            return View(acc);
        }

        [HttpGet]
        public IActionResult Create() => View(new Accommodation());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Accommodation model, IFormFile? mainImage)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = _userManager.GetUserId(User);
            model.OwnerId = userId!;
            model.Status = AccommodationStatus.Pending;
            model.Slug = model.Name.ToLower().Replace(" ", "-").Replace("đ","d");
            // Ensure unique slug
            var existing = await _context.Accommodations.AnyAsync(a => a.Slug == model.Slug);
            if (existing) model.Slug += "-" + Random.Shared.Next(1000, 9999);

            _context.Accommodations.Add(model);
            await _context.SaveChangesAsync();

            if (mainImage != null)
            {
                var url = await _fileUploadService.UploadAccommodationImageAsync(mainImage);
                _context.AccommodationImages.Add(new AccommodationImage { AccommodationId = model.Id, ImageUrl = url, IsMain = true });
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Đã tạo nơi lưu trú. Vui lòng gửi duyệt để hiển thị public.";
            return RedirectToAction("MyAccommodation");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var acc = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == id && a.OwnerId == userId);
            if (acc == null) return NotFound();
            return View(acc);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Accommodation model)
        {
            var userId = _userManager.GetUserId(User);
            var acc = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == model.Id && a.OwnerId == userId);
            if (acc == null) return NotFound();

            acc.Name = model.Name;
            acc.Description = model.Description;
            acc.Address = model.Address;
            acc.City = model.City;
            acc.Province = model.Province;
            acc.AccommodationType = model.AccommodationType;
            acc.StarRating = model.StarRating;
            acc.Phone = model.Phone;
            acc.Email = model.Email;
            acc.CheckInTime = model.CheckInTime;
            acc.CheckOutTime = model.CheckOutTime;
            acc.CancellationPolicy = model.CancellationPolicy;
            acc.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật thông tin.";
            return RedirectToAction("MyAccommodation");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(int accommodationId, List<IFormFile> images)
        {
            var userId = _userManager.GetUserId(User);
            var acc = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == accommodationId && a.OwnerId == userId);
            if (acc == null) return NotFound();

            foreach (var img in images)
            {
                var url = await _fileUploadService.UploadAccommodationImageAsync(img);
                _context.AccommodationImages.Add(new AccommodationImage { AccommodationId = accommodationId, ImageUrl = url });
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã upload {images.Count} ảnh.";
            return RedirectToAction("MyAccommodation");
        }
    }
}
