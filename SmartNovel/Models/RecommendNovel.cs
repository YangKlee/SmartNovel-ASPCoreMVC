using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class RecommendNovel
{
    public string RecommendId { get; set; } = null!;

    public string NovelId { get; set; } = null!;

    public string Uid { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual Novel Novel { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
