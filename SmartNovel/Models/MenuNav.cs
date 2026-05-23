using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class MenuNav
{
    public int Id { get; set; }

    public string RoleId { get; set; } = null!;

    public string? IconBootstrap { get; set; }

    public string Content { get; set; } = null!;

    public int? ParentId { get; set; }

    public string? UrlLink { get; set; }

    public int? Slots { get; set; }

    public virtual ICollection<MenuNav> InverseParent { get; set; } = new List<MenuNav>();

    public virtual MenuNav? Parent { get; set; }

    public virtual Role Role { get; set; } = null!;
}
