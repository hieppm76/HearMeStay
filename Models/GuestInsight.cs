using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HearMeStay.Models.Enums;

namespace HearMeStay.Models
{
    /// <summary>
    /// AI analysis result from guest preference data.
    /// Contains summary, priority, and links to tags/tasks/upsell.
    /// </summary>
    public class GuestInsight
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GuestPreferenceId { get; set; }

        [ForeignKey("GuestPreferenceId")]
        public virtual GuestPreference GuestPreference { get; set; } = null!;

        [Required]
        [Display(Name = "Tóm tắt AI")]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mức độ ưu tiên")]
        public PriorityLevel PriorityLevel { get; set; }

        [Display(Name = "Kết quả AI (JSON)")]
        public string? AiJsonResult { get; set; }

        [Display(Name = "Thông điệp xác nhận")]
        public string? HotelConfirmationMessage { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<GuestInsightTag> Tags { get; set; } = new List<GuestInsightTag>();
        public virtual ICollection<GuestTask> Tasks { get; set; } = new List<GuestTask>();
        public virtual ICollection<UpsellSuggestion> UpsellSuggestions { get; set; } = new List<UpsellSuggestion>();
    }
}
