using System;
using System.Collections.Generic;

namespace SmartNovel.Models;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Slug { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Novel> Novels { get; set; } = new List<Novel>();

    public virtual ICollection<User> Uids { get; set; } = new List<User>();
}
