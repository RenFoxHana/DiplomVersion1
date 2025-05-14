namespace DiplomVersion1.Model;

public partial class Key
{
    public int IdKey { get; set; }

    public string AudienceNumber { get; set; } = null!;

    public int? IdDepartment { get; set; }

    public virtual Department? IdDepartmentNavigation { get; set; }

    public int? IdInstitute { get; set; }

    public virtual Institute? IdInstituteNavigation { get; set; }
    public string? QrCodeBase64 { get; set; }

    public virtual ICollection<LogOfIssuingKey> LogOfIssuingKeys { get; set; } = new List<LogOfIssuingKey>();
}
