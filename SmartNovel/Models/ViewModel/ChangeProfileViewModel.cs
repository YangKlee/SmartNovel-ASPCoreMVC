using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SmartNovel.Models.ViewModel
{
    public class ChangeProfileViewModel
    {
        public string? CurrentImage { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ảnh mới")]
        [Display(Name = "Ảnh đại diện mới")]
        public IFormFile? NewImage { get; set; }
    }
}
