namespace IStudyAPI.Data;

public class CertificateModel
{
    public string Title { get; set; } = null!;

    public string FileB64 { get; set; } = null!;

    public int CertificateLevelId { get; set; }

    public List<CertificateOwnerModel> Owners { get; } = null!;
}

