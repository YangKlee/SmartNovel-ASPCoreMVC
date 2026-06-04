namespace SmartNovel.Models.ViewModel
{
    public class NovelManagermentViewModel
    {
        public string NovelId { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? AgeRating { get; set; }
        public string? ImageNovelUrl { get; set; }
        public string? ImageBanerNovelUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ViewCount { get; set; }
        public int? LikeCount { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; } 
        public int? countChapter { get; set; }
        public int? countChapterPublic { get; set; }
        public int? countChapterDraft { get; set; }
        public int? countChapterRemove { get; set; }
        public double novelRating { set; get; }
        public ICollection<Category> categories { set; get; }
    }
}
