namespace IStudyAPI.Data;

public class CertificateOwnerType
{
    public int Id { get; set; }

    public string OwnerType { get; set; } = null!;

    public virtual ICollection<CertificateOwner> CertificateOwners { get; set; } = new List<CertificateOwner>();
}