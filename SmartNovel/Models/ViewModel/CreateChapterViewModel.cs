namespace SmartNovel.Models.ViewModel
{
    using System.ComponentModel.DataAnnotations;

    public class CreateChapterViewModel
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được quá 255 ký tự")]
        public string Title { set; get; }
        
        [Required(ErrorMessage = "Thứ tự không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự chương phải >= 1")]
        public int Order { set; get; }

        public string? Description { set; get; }
            
        [Required(ErrorMessage = "Trạng thái bắt buộc chọn")]
        public string? Status { set; get; }
        public bool AllowComment { set; get; }
        public string? Content { set; get; }
    }
}
