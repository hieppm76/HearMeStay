using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HearMeStay.Models.Enums;

namespace HearMeStay.Models
{
    public class GuestInsightTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GuestInsightId { get; set; }

        [ForeignKey("GuestInsightId")]
        public virtual GuestInsight GuestInsight { get; set; } = null!;

        [Required]
        [Display(Name = "Danh mục")]
        public TagCategory Category { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên thẻ")]
        public string TagName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mức độ")]
        public SeverityLevel Severity { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
