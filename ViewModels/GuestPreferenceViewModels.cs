using System.ComponentModel.DataAnnotations;
using HearMeStay.Models;

namespace HearMeStay.ViewModels
{
    public class GuestPreferenceCreateViewModel
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string AccommodationName { get; set; } = string.Empty;

        [Display(Name = "Ghi chú tự do")]
        public string? RawText { get; set; }

        [Display(Name = "Có dị ứng thực phẩm")]
        public bool HasFoodAllergy { get; set; }

        [Display(Name = "Chi tiết dị ứng")]
        public string? FoodAllergyDetail { get; set; }

        [Display(Name = "Chế độ ăn")]
        public string? DietPreference { get; set; }

        [Display(Name = "Yêu cầu phòng")]
        public string? RoomPreference { get; set; }

        [Display(Name = "Ghi chú sức khỏe")]
        public string? HealthNote { get; set; }

        [Display(Name = "Dịp đặc biệt")]
        public string? SpecialOccasion { get; set; }

        [Display(Name = "Mục đích chuyến đi")]
        public string? TravelPurpose { get; set; }

        [Display(Name = "Hoạt động quan tâm")]
        public string? ActivityInterest { get; set; }

        [Display(Name = "Cần đưa đón sân bay")]
        public bool NeedAirportPickup { get; set; }

        [Display(Name = "Cần nhận phòng sớm")]
        public bool NeedEarlyCheckIn { get; set; }

        [Display(Name = "Cần trang trí phòng")]
        public bool NeedDecoration { get; set; }

        [Display(Name = "Đồng ý chia sẻ thông tin với nơi lưu trú")]
        public bool ConsentToShareWithHotel { get; set; }

        [Display(Name = "Lưu các sở thích này vào Hồ sơ của tôi cho các chuyến đi sau")]
        public bool SaveToProfile { get; set; }

        [Display(Name = "Đồng ý lưu thông tin sức khỏe (Dị ứng, y tế) vào hồ sơ lâu dài")]
        public bool ConsentToStoreHealthNotes { get; set; }

        public UserPreferenceProfile? SavedProfile { get; set; }
    }

    public class ReviewCreateViewModel
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string AccommodationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng đánh giá.")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        [Display(Name = "Đánh giá chung")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Vui lòng đánh giá cá nhân hóa.")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        [Display(Name = "Mức độ hài lòng với trải nghiệm cá nhân hóa")]
        public int PersonalizationRating { get; set; }

        [Display(Name = "Nơi lưu trú có chuẩn bị đúng nhu cầu của bạn không?")]
        public bool DidHotelPrepareWell { get; set; }

        [Display(Name = "Nhận xét")]
        public string? Comment { get; set; }
    }
}
