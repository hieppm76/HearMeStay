using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HearMeStay.Models.Enums;

namespace HearMeStay.Models
{
    /// <summary>
    /// Tasks that hotel staff need to prepare before guest arrival.
    /// Grouped by department (Reception, Housekeeping, Kitchen, etc.)
    /// </summary>
    public class GuestTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GuestInsightId { get; set; }

        [ForeignKey("GuestInsightId")]
        public virtual GuestInsight GuestInsight { get; set; } = null!;

        [Required]
        [Display(Name = "Bộ phận")]
        public Department Department { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Trạng thái")]
        public GuestTaskStatus TaskStatus { get; set; } = GuestTaskStatus.Pending;

        [Display(Name = "Ghi chú đối tác")]
        public string? PartnerNote { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }
    }
}
