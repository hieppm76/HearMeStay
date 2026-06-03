using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HearMeStay.Models
{
    /// <summary>
    /// Guest preference data submitted before check-in.
    /// Only allowed when BookingStatus = Confirmed.
    /// Health/allergy info is restricted to the owning partner and admin.
    /// </summary>
    public class GuestPreference
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

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

        [Display(Name = "Đồng ý chia sẻ với nơi lưu trú")]
        public bool ConsentToShareWithHotel { get; set; }

        [Display(Name = "Trạng thái yêu cầu đặc biệt")]
        public HearMeStay.Models.Enums.SpecialRequestStatus SpecialRequestStatus { get; set; } = HearMeStay.Models.Enums.SpecialRequestStatus.Pending;

        [Display(Name = "Phản hồi của khách sạn")]
        public string? PartnerSpecialRequestNote { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "AI xử lý lúc")]
        public DateTime? AiProcessedAt { get; set; }

        // Navigation
        public virtual GuestInsight? GuestInsight { get; set; }
    }
}
