using System.ComponentModel.DataAnnotations.Schema;

namespace HearMeStay.Models
{
    /// <summary>
    /// Many-to-many join table between Accommodation and Amenity.
    /// Uses composite key (AccommodationId + AmenityId).
    /// </summary>
    public class AccommodationAmenity
    {
        public int AccommodationId { get; set; }

        [ForeignKey("AccommodationId")]
        public virtual Accommodation Accommodation { get; set; } = null!;

        public int AmenityId { get; set; }

        [ForeignKey("AmenityId")]
        public virtual Amenity Amenity { get; set; } = null!;
    }
}
