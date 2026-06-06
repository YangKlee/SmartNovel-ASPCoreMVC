using SmartNovel.Models;

namespace SmartNovel.ViewModels
{
    public class NovelSearchViewModel
    {
        public string? Keyword { get; set; }

        public string? Status { get; set; }

        public string? AuthorId { get; set; }

        public string? SortBy { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public bool IsSearching { get; set; }

        public double? MinRating { get; set; }
        public List<string> SelectedCategories { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public List<User> Authors { get; set; } = new();

        public List<Novel> Novels { get; set; } = new();
    }
}