namespace SmartNovel.Models.ViewModel
{
    public class CommentManagermentViewModel
    {
        public string Keyword { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItem { get; set; }
        public int TotalPage
        {
            get
            {
                if (PageSize == 0) return 0;
                return (int)Math.Ceiling((double)TotalItem / PageSize);
            }
        }
        public List<Comment> Comments { get; set; }
    }
}
