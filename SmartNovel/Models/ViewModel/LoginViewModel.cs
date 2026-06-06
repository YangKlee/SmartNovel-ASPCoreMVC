using System.ComponentModel.DataAnnotations;

namespace SmartNovel.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản.")]
        public string? txtUsername { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string? txtPassword { get; set; }
    }
}
