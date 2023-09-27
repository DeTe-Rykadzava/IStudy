namespace IStudyAPI.Data;

public class CertificateLevel
{
    public int Id { get; set; }

    public string Level { get; set; } = null!;

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}