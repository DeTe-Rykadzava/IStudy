namespace IStudyAPI.Data;

public class CertificateOwner
{
    public int Id { get; set; }

    public int CertificateId { get; set; }

    public string CertificateOwnerUserId { get; set; } = null!;

    public int CertificateOwnerType { get; set; }

    public virtual Certificate Certificate { get; set; } = null!;

    public virtual CertificateOwnerType CertificateOwnerTypeNavigation { get; set; } = null!;

    public virtual User CertificateOwnerUser { get; set; } = null!;
}