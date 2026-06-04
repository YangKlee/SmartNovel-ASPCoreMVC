using SmartNovel.Models;
namespace SmartNovel.Models.ViewModel
{
    public class NovelViewModel
    {
        public string NovelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageNovelUrl { get; set; }
        public string AuthorName { get; set; }
        public string Status { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public int CountChapterPublic { get; set; }
        public int CountChapterDraf { get; set; } 
        public int CountChapterRemove { get; set; }
        public int ViewCount { get; set; }
        public double NovelRating { get; set; } 
        public int LikeCount { get; set; }
    }
}
