using SmartNovel.Models;

namespace SmartNovel.Models.ViewModels
{
    public class NovelDetailVM
    {
        public Novel Novel { get; set; }

        public User? Author { get; set; }

        public List<Chapter> Chapters { get; set; } = new();

        public List<string> Categories { get; set; } = new();

        public bool IsFollowing { get; set; }

        public int FollowCount { get; set; }

        public double AverageRating { get; set; }

        public double? UserRating { get; set; }

        public int TotalRatings { get; set; }

        public bool IsBlockedAuthor { get; set; }

        public bool IsAuthorBlocked { get; set; } 
    }
}