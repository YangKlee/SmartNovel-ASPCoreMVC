namespace SmartNovel.Models.ViewModel
{
    public class CommentReaderViewModel
    {
        public string SelectedNovel { get; set; }
        public string SelectedChapter { set; get; }
        public string Keyword { get; set; }
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
        public List<SmartNovel.Models.Comment> Comments { get; set; }
    }
}
