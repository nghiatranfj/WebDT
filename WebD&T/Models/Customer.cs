using System.ComponentModel.DataAnnotations;

namespace WebD_T.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "*")]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "*")]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Ảnh đại diện")]
        public string? Img { get; set; }   // nullable

        // 🚨 Không dùng DateOnly vì SQL Server không hỗ trợ!
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }   // đổi sang DateTime?

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [MaxLength(200)]
        public string Password { get; set; } = string.Empty;

        // RandomKey phục vụ hash
        public string? RandomKey { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime RegisterAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;     // Đổi int → bool

        [Display(Name = "Quyền")]
        public int Role { get; set; } = 0;   // 0=customer, 1=admin...
    }
}
