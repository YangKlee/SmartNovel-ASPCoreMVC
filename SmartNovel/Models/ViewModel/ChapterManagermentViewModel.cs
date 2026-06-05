namespace SmartNovel.Models.ViewModel
{
    public class ChapterManagermentViewModel
    {
        public List<Chapter>? chapters { set; get; }
        public List<NovelViewModel>? Novels { get; set; }
        public string? SelectedNovelId { get; set; }
        public NovelViewModel? SelectedNovel { get; set; }
        public string? SelectedStatus { get; set; }
        public string? Keyword { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItem { get; set; }
        public int TotalPage
        {
            get
            {
                if (PageSize == 0) return 0; // Tránh lỗi chia cho 0
                return (int)Math.Ceiling((double)TotalItem / PageSize);
            }
        }
    }
}
