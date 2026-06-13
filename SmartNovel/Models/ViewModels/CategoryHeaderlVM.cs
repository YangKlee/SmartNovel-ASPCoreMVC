using SmartNovel.Models;

namespace SmartNovel.ViewModels
{
    public class CategoryHeaderVM
    {
        public string Slug { get; set; } = "";

        public string CategoryName { get; set; } = "";

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public List<Novel> Novels { get; set; }
            = new();
    }
}