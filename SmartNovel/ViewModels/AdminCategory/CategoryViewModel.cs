using System.ComponentModel.DataAnnotations;

namespace SmartNovel.ViewModels.AdminCategory
{
    public class CategoryViewModel
    {
        public string? CategoryId { get; set; }

        [Required(ErrorMessage = "Tên thể loại không được để trống")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Slug { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public string Status { get; set; } = "active";
    }
}
