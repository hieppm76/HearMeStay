using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HearMeStay.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        public int AccommodationId { get; set; }

        [ForeignKey("AccommodationId")]
        public virtual Accommodation Accommodation { get; set; } = null!;

        [Required]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        [Display(Name = "Đánh giá chung")]
        public int Rating { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Đánh giá cá nhân hóa phải từ 1 đến 5 sao.")]
        [Display(Name = "Đánh giá cá nhân hóa")]
        public int PersonalizationRating { get; set; }

        [Display(Name = "Nơi lưu trú chuẩn bị tốt?")]
        public bool DidHotelPrepareWell { get; set; }

        [Display(Name = "Nhận xét")]
        public string? Comment { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Hiển thị")]
        public bool IsVisible { get; set; } = true;
    }
}
