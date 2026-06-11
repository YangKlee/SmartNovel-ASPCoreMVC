namespace SmartNovel.Models.ViewModel
{
    public class NovelHistoryViewModel
    {
        public Chapter chapterView { set; get; }
        public Novel novelInfo { set; get; }
    }
    public class HistoryViewModel
    {
        public NovelHistoryViewModel history { set; get; }
        public DateTime? timeView { set; get; }
    }
}
