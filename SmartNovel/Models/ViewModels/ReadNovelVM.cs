namespace SmartNovel.Models.ViewModels
{
    public class ReadNovelVM
    {
        public string NovelId { get; set; } = string.Empty;

        public string ChapterId { get; set; } = string.Empty;

        public string NovelTitle { get; set; } = string.Empty;

        public string ChapterTitle { get; set; } = string.Empty;

        public int ChapterOrder { get; set; }

        public string HtmlContent { get; set; } = string.Empty;

        public string? PrevChapterId { get; set; }

        public string? NextChapterId { get; set; }

        public string? PrevChapterTitle { get; set; }

        public string? NextChapterTitle { get; set; }

        public List<Chapter>? AllChapters { get; set; } = new();

        public List<Comment>? Comments { get; set; } = new();
    }
}