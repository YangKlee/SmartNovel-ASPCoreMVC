using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SmartNovel.Models.ViewModel
{
    public class CreateNovelViewModel
    {
        public string? NovelId { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        [MaxLength(255, ErrorMessage = "Tiêu đề tối đa 255 ký tự.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Độ tuổi là bắt buộc.")]
        public string AgeRating { get; set; } = "6";

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public string Status { get; set; } = "Public";

        public List<string>? Genres { get; set; } = new List<string>();

        public IFormFile? CoverImage { get; set; }
        public IFormFile? BannerImage { get; set; }

        // Optional list to hold available genres from database to render checkboxes
        // public List<CategoryViewModel>? AvailableGenres { get; set; }
    }
}
