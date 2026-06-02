using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HearMeStay.Models
{
    public class RoomImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [ForeignKey("RoomTypeId")]
        public virtual RoomType RoomType { get; set; } = null!;

        [Required]
        [Display(Name = "Đường dẫn ảnh")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Ảnh chính")]
        public bool IsMain { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
