using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HearMeStay.Models.Enums;

namespace HearMeStay.Models
{
    public class UpsellSuggestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GuestInsightId { get; set; }

        [ForeignKey("GuestInsightId")]
        public virtual GuestInsight GuestInsight { get; set; } = null!;

        public int? AddOnServiceId { get; set; }

        [ForeignKey("AddOnServiceId")]
        public virtual AddOnService? AddOnService { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Lý do gợi ý")]
        public string Reason { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá ước tính")]
        public decimal? EstimatedPrice { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public UpsellStatus Status { get; set; } = UpsellStatus.Suggested;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
