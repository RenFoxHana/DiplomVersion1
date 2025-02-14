namespace DiplomVersion1.Model;

public partial class LogOfIssuingKey
{
    public int IdEntry { get; set; }

    public int IdEmployee { get; set; }

    public int IdKey { get; set; }

    public int IdWatchman { get; set; }

    public DateTime DateTimeOfIssue { get; set; }

    public DateTime? DateTimeOfDelivery { get; set; }

    public virtual Employee IdEmployeeNavigation { get; set; } = null!;

    public virtual Key IdKeyNavigation { get; set; } = null!;

    public virtual Watchman IdWatchmanNavigation { get; set; } = null!;
}
