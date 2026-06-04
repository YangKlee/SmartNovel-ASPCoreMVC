namespace SmartNovel.Models.ViewModel
{
    public class NovelManagermentViewModel
    {
        public List<NovelViewModel> novels { set; get; }
        public string SelectedStatus { get; set; }
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

    }
}
