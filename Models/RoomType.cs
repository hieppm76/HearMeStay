using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HearMeStay.Models
{
    /// <summary>
    /// Room type within an accommodation. Contains AI-friendly fields
    /// (IsQuietRoom, SupportsVeganMeal, etc.) for smart matching.
    /// </summary>
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        [ForeignKey("AccommodationId")]
        public virtual Accommodation Accommodation { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên phòng.")]
        [StringLength(200)]
        [Display(Name = "Tên phòng")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá mỗi đêm.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá mỗi đêm")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Sức chứa phải từ 1 đến 50.")]
        [Display(Name = "Sức chứa")]
        public int Capacity { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Tổng số phòng phải từ 0 đến 1000.")]
        [Display(Name = "Tổng số phòng")]
        public int TotalRooms { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Số phòng trống phải từ 0 đến 1000.")]
        [Display(Name = "Phòng còn trống")]
        public int AvailableRooms { get; set; }

        [StringLength(100)]
        [Display(Name = "Loại giường")]
        public string BedType { get; set; } = string.Empty;

        [Display(Name = "Diện tích (m²)")]
        public double? RoomSize { get; set; }

        [Display(Name = "Phòng tắm riêng")]
        public bool HasPrivateBathroom { get; set; } = true;

        // AI-friendly room features
        [Display(Name = "Phòng yên tĩnh")]
        public bool IsQuietRoom { get; set; }

        [Display(Name = "Hỗ trợ ăn chay")]
        public bool SupportsVeganMeal { get; set; }

        [Display(Name = "Hỗ trợ khách dị ứng")]
        public bool SupportsAllergyRequest { get; set; }

        [Display(Name = "Không mùi nồng")]
        public bool NoStrongScentAvailable { get; set; }

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
