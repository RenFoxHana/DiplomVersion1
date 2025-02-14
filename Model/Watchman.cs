namespace DiplomVersion1.Model;
public partial class Watchman
{
    public int IdWatchman { get; set; }

    public string LastNameWm { get; set; } = null!;

    public string FirstNameWm { get; set; } = null!;

    public string? PatronymicWm { get; set; }

    public string WmLogin { get; set; } = null!;

    public string WmPassword { get; set; } = null!;

    public virtual ICollection<LogOfIssuingKey> LogOfIssuingKeys { get; set; } = new List<LogOfIssuingKey>();

    public string FullName => $"{LastNameWm} {FirstNameWm} {PatronymicWm}";
}
