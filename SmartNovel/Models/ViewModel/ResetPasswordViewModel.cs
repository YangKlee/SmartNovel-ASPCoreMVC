using System.ComponentModel.DataAnnotations;

namespace SmartNovel.Models.ViewModel
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có tối thiểu 6 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string txtNewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
        [DataType(DataType.Password)]
        [Compare("txtNewPassword", ErrorMessage = "Mật khẩu không khớp.")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string txtConfirmPassword { get; set; }
    }
}
