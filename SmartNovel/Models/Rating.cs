using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class Rating
{
    public string Uid { get; set; } = null!;

    public string NovelId { get; set; } = null!;

    public double RatingPoint { get; set; }

    public virtual Novel Novel { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
