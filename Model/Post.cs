using System;
using System.Collections.Generic;

namespace DiplomVersion1.Model;

public partial class Post
{
    public int IdPost { get; set; }

    public string NamePost { get; set; } = null!;

    public Post ShallowCopy()
    {
        return (Post)this.MemberwiseClone();
    }
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
