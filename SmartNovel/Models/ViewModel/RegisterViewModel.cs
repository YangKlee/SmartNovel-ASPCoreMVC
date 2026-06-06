using System.ComponentModel.DataAnnotations;

namespace SmartNovel.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên tài khoản phải từ 3 đến 50 ký tự.")]
        [Display(Name = "Tên tài khoản")]
        public string txtUsername { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập email hợp lệ.")]
        [Display(Name = "Email")]
        public string txtEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị.")]
        [StringLength(100, ErrorMessage = "Tên hiển thị không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên hiển thị")]
        public string txtDisplayName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [RegularExpression(@"^([0-9]{10,11})$", ErrorMessage = "Số điện thoại phải chứa 10-11 chữ số.")]
        [Display(Name = "Số điện thoại")]
        public string txtPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có tối thiểu 6 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string txtPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare("txtPassword", ErrorMessage = "Mật khẩu không khớp.")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string txtConfirmPassword { get; set; }
    }
}
