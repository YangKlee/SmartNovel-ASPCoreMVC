//using System.ComponentModel.DataAnnotations;

//namespace SmartNovel.ViewModels.AdminUser;

//public class CreateUserViewModel
//{
//    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
//    [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
//    public string FullName { get; set; }

//    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
//    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập không được chứa khoảng trắng hoặc ký tự đặc biệt.")]
//    [MaxLength(50)]
//    public string Username { get; set; }

//    [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
//    [EmailAddress(ErrorMessage = "Email không hợp lệ (Ví dụ đúng: abc@domain.com).")]
//    public string Email { get; set; }

//    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
//    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
//    public string Password { get; set; }

//    [Required(ErrorMessage = "Vui lòng chọn quyền cho người dùng.")]
//    public int RoleID { get; set; } 
//}