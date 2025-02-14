namespace DiplomVersion1.Model;

public partial class Employee
{
    public int IdEmployee { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public int? IdPost { get; set; }

    public int? IdDepartment { get; set; }

    public virtual Department? IdDepartmentNavigation { get; set; }

    public virtual Post? IdPostNavigation { get; set; }

    public virtual ICollection<LogOfIssuingKey> LogOfIssuingKeys { get; set; } = new List<LogOfIssuingKey>();

    public string FullName => $"{LastName} {FirstName} {Patronymic}";
}
