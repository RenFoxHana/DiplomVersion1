namespace DiplomVersion1.Model;

public partial class Institute
{
    public int IdInstitute { get; set; }

    public string NameIns { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    public virtual ICollection<Key> Keys { get; set; } = new List<Key>();
}
