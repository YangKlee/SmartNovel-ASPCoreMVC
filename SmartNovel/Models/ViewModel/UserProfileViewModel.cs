using System.ComponentModel.DataAnnotations;

namespace SmartNovel.Models.ViewModel
{
    public class UserProfileViewModel
    {
            [Required(ErrorMessage = "Tên hiển thị không được để trống")]
            [StringLength(50, ErrorMessage = "Tên hiển thị không quá 50 ký tự")]
            [Display(Name = "Tên hiển thị")]
            public string DisplayName { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
            [DataType(DataType.Date)]
            [Display(Name = "Ngày sinh")]
            public DateOnly? Birthday { get; set; }

            [Required(ErrorMessage = "Số di động không được để trống")]
            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            [RegularExpression(@"^(0[3|5|7|8|9])+([0-8]{8})\b$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
            [Display(Name = "Di động")]
            public string Phone { get; set; }

    }
}
