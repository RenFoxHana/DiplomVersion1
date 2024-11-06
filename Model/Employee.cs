using System;
using System.Collections.Generic;

namespace DiplomVersion1.Model;

public partial class Employee
{
    public int IdEmployee { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public int IdPost { get; set; }

    public int IdDepartment { get; set; }

    public virtual Department IdDepartmentNavigation { get; set; } = null!;

    public virtual Post IdPostNavigation { get; set; } = null!;
}
