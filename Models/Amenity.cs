using System.ComponentModel.DataAnnotations;

namespace HearMeStay.Models
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên tiện ích")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Icon CSS class")]
        public string? IconClass { get; set; }

        // Navigation
        public virtual ICollection<AccommodationAmenity> AccommodationAmenities { get; set; } = new List<AccommodationAmenity>();
    }
}
