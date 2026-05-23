using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class Novel
{
    public string NovelId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string AgeRating { get; set; } = null!;

    public string? ImageNovelUrl { get; set; }

    public string? ImageBanerNovelUrl { get; set; }

    public string Status { get; set; } = null!;

    public string Uid { get; set; } = null!;

    public int? ViewCount { get; set; }

    public int? LikeCount { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<HistoryReader> HistoryReaders { get; set; } = new List<HistoryReader>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<RecommendNovel> RecommendNovels { get; set; } = new List<RecommendNovel>();

    public virtual ICollection<ReportTicket> ReportTickets { get; set; } = new List<ReportTicket>();

    public virtual User UidNavigation { get; set; } = null!;

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<User> Uids { get; set; } = new List<User>();
}
