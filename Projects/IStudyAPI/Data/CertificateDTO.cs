namespace IStudyAPI.Data;

public class CertificateDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string AddedUser { get; set; } = null!;
    
    public DateTime CreateDate { get; set; }

    public string CertificateLevel { get; set; }
}