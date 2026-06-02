using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HearMeStay.Models.Enums;

namespace HearMeStay.Models
{
    public class CommissionTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        [Required]
        public int AccommodationId { get; set; }

        [ForeignKey("AccommodationId")]
        public virtual Accommodation Accommodation { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Tỷ lệ hoa hồng")]
        public decimal CommissionRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Hoa hồng")]
        public decimal CommissionAmount { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public CommissionStatus Status { get; set; } = CommissionStatus.Pending;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày thanh toán")]
        public DateTime? PaidAt { get; set; }
    }
}
