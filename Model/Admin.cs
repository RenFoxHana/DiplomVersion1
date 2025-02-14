namespace DiplomVersion1.Model;

public partial class Admin
{
    public int IdAdmin { get; set; }

    public string LastNameAdmin { get; set; } = null!;

    public string FirstNameAdmin { get; set; } = null!;

    public string? PatronymicAdmin { get; set; }

    public string AdminLogin { get; set; } = null!;

    public string AdminPassword { get; set; } = null!;
}
