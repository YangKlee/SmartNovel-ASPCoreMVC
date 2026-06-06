using System.ComponentModel.DataAnnotations;

namespace SmartNovel.ViewModels.AdminUser
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string Displayname { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập không được chứa khoảng trắng hoặc ký tự đặc biệt.")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền cho người dùng.")]
        public string RoleID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái cho người dùng.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm ban đầu.")]
        [Range(0, 1000000, ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 1,000,000.")]
        public int CreatorPoint { get; set; } = 0; 
    }
}