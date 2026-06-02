using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HearMeStay.Models.Enums;

namespace HearMeStay.Models
{
    public class AddOnService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        [ForeignKey("AccommodationId")]
        public virtual Accommodation Accommodation { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ.")]
        [StringLength(200)]
        [Display(Name = "Tên dịch vụ")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá dịch vụ")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Loại dịch vụ")]
        public ServiceType ServiceType { get; set; }

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
