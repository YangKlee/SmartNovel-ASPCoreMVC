using System.ComponentModel.DataAnnotations;

namespace SmartNovel.Models.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email để khôi phục mật khẩu.")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập email hợp lệ.")]
        [Display(Name = "Email")]
        public string txtEmail { get; set; }
    }
}
