using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class Chapter
{
    public string ChapterId { get; set; } = null!;

    public int ChaperOrder { get; set; }

    public string NovelId { get; set; } = null!;

    public string ChapterTitle { get; set; } = null!;

    public string? SummaryChapter { get; set; }

    public string? ChapterFileUrl { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public bool? AllowComment { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<HistoryReader> HistoryReaders { get; set; } = new List<HistoryReader>();

    public virtual Novel Novel { get; set; } = null!;

    public virtual ICollection<ReportTicket> ReportTickets { get; set; } = new List<ReportTicket>();
}
