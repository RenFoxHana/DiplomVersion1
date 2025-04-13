namespace DiplomVersion1.Model;

public partial class Institute
{
    public int IdInstitute { get; set; }

    public string NameIns { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    public virtual ICollection<Key> Keys { get; set; } = new List<Key>();

    //Вычисляемое свойство для отображения кафедр
    public string DisplayedDepartments
    {
        get
        {
            if (Departments == null || !Departments.Any())
                return "Нет кафедр";

            return string.Join(", ", Departments.Select(d => d.NameDep));
        }
    }
}
