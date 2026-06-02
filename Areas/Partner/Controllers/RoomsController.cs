using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Areas.Partner.Controllers
{
    [Area("Partner")]
    [Authorize(Roles = "HotelPartner")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public RoomsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFileUploadService fileUploadService)
        { _context = context; _userManager = userManager; _fileUploadService = fileUploadService; }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var rooms = await _context.RoomTypes.Where(r => r.Accommodation.OwnerId == userId).Include(r => r.Images).Include(r => r.Accommodation).OrderBy(r => r.Name).ToListAsync();
            return View(rooms);
        }

        [HttpGet] public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var acc = await _context.Accommodations.FirstOrDefaultAsync(a => a.OwnerId == userId);
            if (acc == null) { TempData["Error"] = "Vui lòng tạo nơi lưu trú trước."; return RedirectToAction("MyAccommodation", "Accommodations"); }
            ViewBag.AccommodationId = acc.Id;
            return View(new RoomType { AccommodationId = acc.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomType model, IFormFile? mainImage)
        {
            var userId = _userManager.GetUserId(User);
            var acc = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == model.AccommodationId && a.OwnerId == userId);
            if (acc == null) return NotFound();
            if (!ModelState.IsValid) { ViewBag.AccommodationId = acc.Id; return View(model); }
            _context.RoomTypes.Add(model);
            await _context.SaveChangesAsync();
            if (mainImage != null)
            {
                var url = await _fileUploadService.UploadRoomImageAsync(mainImage);
                _context.RoomImages.Add(new RoomImage { RoomTypeId = model.Id, ImageUrl = url, IsMain = true });
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Đã thêm loại phòng.";
            return RedirectToAction("Index");
        }

        [HttpGet] public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var room = await _context.RoomTypes.Include(r => r.Accommodation).FirstOrDefaultAsync(r => r.Id == id && r.Accommodation.OwnerId == userId);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomType model)
        {
            var userId = _userManager.GetUserId(User);
            var room = await _context.RoomTypes.Include(r => r.Accommodation).FirstOrDefaultAsync(r => r.Id == model.Id && r.Accommodation.OwnerId == userId);
            if (room == null) return NotFound();
            room.Name = model.Name; room.Description = model.Description; room.PricePerNight = model.PricePerNight;
            room.Capacity = model.Capacity; room.TotalRooms = model.TotalRooms; room.AvailableRooms = model.AvailableRooms;
            room.BedType = model.BedType; room.RoomSize = model.RoomSize;
            room.IsQuietRoom = model.IsQuietRoom; room.SupportsVeganMeal = model.SupportsVeganMeal;
            room.SupportsAllergyRequest = model.SupportsAllergyRequest; room.NoStrongScentAvailable = model.NoStrongScentAvailable;
            room.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật phòng.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var room = await _context.RoomTypes.Include(r => r.Accommodation).FirstOrDefaultAsync(r => r.Id == id && r.Accommodation.OwnerId == userId);
            if (room == null) return NotFound();
            _context.RoomTypes.Remove(room);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa phòng.";
            return RedirectToAction("Index");
        }
    }
}
