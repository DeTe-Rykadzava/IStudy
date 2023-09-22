using System;
using System.Collections.Generic;

namespace IStudyAPI.Data;

public partial class Class
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
