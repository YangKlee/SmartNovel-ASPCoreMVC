using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class HistoryReader
{
    public string ReadSessionId { get; set; } = null!;

    public string Uid { get; set; } = null!;

    public string NovelId { get; set; } = null!;

    public string ChapterId { get; set; } = null!;

    public DateTime? TimeReader { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual Novel Novel { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
