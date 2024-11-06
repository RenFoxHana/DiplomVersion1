using System;
using System.Collections.Generic;

namespace DiplomVersion1.Model;

public partial class Institute
{
    public int IdInstitute { get; set; }

    public string NameIns { get; set; } = null!;

    public Institute ShallowCopy()
    {
        return (Institute)this.MemberwiseClone();
    }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
