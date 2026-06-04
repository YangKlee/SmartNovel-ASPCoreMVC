namespace SmartNovel.Models.ViewModel
{
    public class NovelManagermentViewModel
    {
        public List<NovelViewModel> novels { set; get; }
        public string SelectedStatus { get; set; }
        public string Keyword { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItem { get; set; }
        
    }
}
