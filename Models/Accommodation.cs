using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HearMeStay.Models.Enums;

namespace HearMeStay.Models
{
    /// <summary>
    /// Represents a lodging property (hotel, homestay, villa, resort, apartment).
    /// Must be approved by Admin before becoming publicly visible.
    /// </summary>
    public class Accommodation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên nơi lưu trú.")]
        [StringLength(200)]
        [Display(Name = "Tên nơi lưu trú")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả.")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập thành phố.")]
        [StringLength(100)]
        [Display(Name = "Thành phố")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành.")]
        [StringLength(100)]
        [Display(Name = "Tỉnh/Thành")]
        public string Province { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Loại hình")]
        public AccommodationType AccommodationType { get; set; }

        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5.")]
        [Display(Name = "Số sao")]
        public int? StarRating { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        [Display(Name = "Số Zalo")]
        public string? ZaloPhone { get; set; }

        [Required]
        [Display(Name = "Giờ nhận phòng")]
        public TimeSpan CheckInTime { get; set; } = new TimeSpan(14, 0, 0);

        [Required]
        [Display(Name = "Giờ trả phòng")]
        public TimeSpan CheckOutTime { get; set; } = new TimeSpan(12, 0, 0);

        [Display(Name = "Chính sách hủy")]
        public string CancellationPolicy { get; set; } = "Hủy miễn phí trước 24 giờ";

        [Required]
        [Display(Name = "Trạng thái")]
        public AccommodationStatus Status { get; set; } = AccommodationStatus.Pending;

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public virtual ICollection<AccommodationImage> Images { get; set; } = new List<AccommodationImage>();
        public virtual ICollection<AddOnService> AddOnServices { get; set; } = new List<AddOnService>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<AccommodationAmenity> AccommodationAmenities { get; set; } = new List<AccommodationAmenity>();
    }
}
