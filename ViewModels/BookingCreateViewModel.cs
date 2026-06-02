using System.ComponentModel.DataAnnotations;

namespace HearMeStay.ViewModels
{
    public class BookingCreateViewModel
    {
        public int AccommodationId { get; set; }
        public string AccommodationName { get; set; } = string.Empty;

        [Required]
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [Display(Name = "Họ tên khách")]
        public string GuestFullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string GuestEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string GuestPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày nhận phòng")]
        public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày trả phòng")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);

        [Required]
        [Range(1, 50, ErrorMessage = "Số khách phải từ 1 đến 50.")]
        [Display(Name = "Số khách")]
        public int NumberOfGuests { get; set; } = 1;

        [Required]
        [Range(1, 20, ErrorMessage = "Số phòng phải từ 1 đến 20.")]
        [Display(Name = "Số phòng")]
        public int NumberOfRooms { get; set; } = 1;

        [Display(Name = "Ghi chú")]
        public string? GuestNote { get; set; }
    }
}
