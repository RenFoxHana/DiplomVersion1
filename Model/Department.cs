using System;
using System.Collections.Generic;

namespace DiplomVersion1.Model;

public partial class Department
{
    public int IdDepartment { get; set; }

    public string NameDep { get; set; } = null!;

    public int IdInstitute { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Institute IdInstituteNavigation { get; set; } = null!;
}
