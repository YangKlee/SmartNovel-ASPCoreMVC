using System.ComponentModel.DataAnnotations;

namespace SmartNovel.Models.ViewModel
{
    public class UserProfileViewModel
    {
        // UpdateInfo
            [Required(ErrorMessage = "Tên hiển thị không được để trống")]
            [StringLength(50, ErrorMessage = "Tên hiển thị không quá 50 ký tự")]
            [Display(Name = "Tên hiển thị")]
            public string DisplayName { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
            [DataType(DataType.Date)]
            [Display(Name = "Ngày sinh")]
            public DateOnly? Birthday { get; set; }

            [Required(ErrorMessage = "Số di động không được để trống")]
            [Display(Name = "Di động")]
            public string Phone { get; set; }
         // Profile
            public User? User { get; set; }

            public List<Novel> FollowingNovels { get; set; } = new();

            public List<User> FollowingAuthors { get; set; } = new();

            public List<User> Followers { get; set; } = new();

            public List<Novel> PublicNovels { get; set; } = new();

            public int FollowNovelCount { get; set; }

            public int FollowAuthorCount { get; set; }

            public int FollowerCount { get; set; }

            public int TotalNovelCount { get; set; }

            public int TotalViewCount { get; set; }

            public bool IsCurrentUser { get; set; }

            public bool IsBlocked { get; set; }
    }
}
