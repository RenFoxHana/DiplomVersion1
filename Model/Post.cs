namespace DiplomVersion1.Model;

public partial class Post
{
    public int IdPost { get; set; }

    public string NamePost { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
