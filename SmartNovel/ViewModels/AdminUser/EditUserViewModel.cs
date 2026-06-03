using System.ComponentModel.DataAnnotations;

namespace SmartNovel.ViewModels.AdminUser
{
    public class EditUserViewModel
    {
        [Required]
        public string Uid { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string Displayname { get; set; }

        public string? Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền cho người dùng.")]
        public string RoleID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái cho người dùng.")]
        public string Status { get; set; }
    }
}

