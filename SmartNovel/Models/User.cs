using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class User
{
    public string Uid { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public string Password { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string? AvartarUrl { get; set; }

    public string? Phone { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? BannedTime { get; set; }

    public DateTime? TimeOutTime { get; set; }

    public string? TimeOutType { get; set; }

    public int? CreatorPoint { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<HistoryReader> HistoryReaders { get; set; } = new List<HistoryReader>();

    public virtual ICollection<Novel> NovelsNavigation { get; set; } = new List<Novel>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<RecommendNovel> RecommendNovels { get; set; } = new List<RecommendNovel>();

    public virtual ICollection<ReportTicket> ReportTicketRepoterUs { get; set; } = new List<ReportTicket>();

    public virtual ICollection<ReportTicket> ReportTicketResolvedUs { get; set; } = new List<ReportTicket>();

    public virtual ICollection<ReportTicket> ReportTicketTargetUs { get; set; } = new List<ReportTicket>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<User> Authors { get; set; } = new List<User>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<User> FollowerUs { get; set; } = new List<User>();

    public virtual ICollection<Novel> Novels { get; set; } = new List<Novel>();

    public virtual ICollection<User> Uids { get; set; } = new List<User>();

    public virtual ICollection<User> UidsNavigation { get; set; } = new List<User>();
}
