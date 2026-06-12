using HearMeStay.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HearMeStay.Models
{
    public class UserPreferenceTag
    {
        [Key]
        public int Id { get; set; }

        public int UserPreferenceProfileId { get; set; }

        [ForeignKey("UserPreferenceProfileId")]
        public virtual UserPreferenceProfile Profile { get; set; } = null!;

        public TagCategory Category { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TagName { get; set; } = string.Empty;

        public string? Description { get; set; }
        
        public bool IsReusable { get; set; } = true;
        
        public int? CreatedFromBookingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
