using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string RoleDisplayName { get; set; } = null!;

    public string? RoleDescription { get; set; }

    public virtual ICollection<MenuNav> MenuNavs { get; set; } = new List<MenuNav>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
