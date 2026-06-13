using SmartNovel.Models;

namespace SmartNovel.Models.ViewModels
{
    public class FollowingNovelVM
    {
        public List<Novel> Novels { get; set; } = new();
        public List<User> author { set; get; } = new();
    }
}