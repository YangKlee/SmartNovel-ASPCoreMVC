using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class ReportTicket
{
    public string TiketId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string ReasonDetail { get; set; } = null!;

    public string? NovelId { get; set; }

    public string? ChapterId { get; set; }

    public string? CommentId { get; set; }

    public string? TargetUid { get; set; }

    public string RepoterUid { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ResolvedUid { get; set; }

    public DateTime? TimeSend { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual Novel? Novel { get; set; }

    public virtual User RepoterU { get; set; } = null!;

    public virtual User? ResolvedU { get; set; }

    public virtual User? TargetU { get; set; }
}
