namespace SmartNovel.Models.ViewModel
{
    public class AuthorListModel
    {
        public List<User> Authors { get; set; } = new List<User>();
        public string? Keyword { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
