using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HearMeStay.Models
{
    public class UserPreferenceProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        public string? FoodPreferences { get; set; }
        public string? AllergyNotes { get; set; }
        public string? RoomPreferences { get; set; }
        public string? ServicePreferences { get; set; }
        public string? ActivityInterests { get; set; }
        
        public string? HealthNotes { get; set; }
        public bool ConsentToStoreHealthNotes { get; set; }
        
        public bool ConsentToShareWithHotel { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<UserPreferenceTag> Tags { get; set; } = new List<UserPreferenceTag>();
    }
}
