namespace SmartNovel.Models.ViewModels
{
    public class ProfileVM
    {
        public User User { get; set; } = null!;

        public bool IsFollowing { get; set; }

        public bool IsAuthorBlocked { get; set; }

        public List<Novel> PublicNovels { get; set; } = new();

        public List<User> FollowingAuthors { get; set; } = new();

        public List<User> Followers { get; set; } = new();
    }
}
