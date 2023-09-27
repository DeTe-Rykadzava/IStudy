namespace IStudyAPI.Data;

public class CertificateLevelDTO
{
    public int Id { get; set; }

    public string Level { get; set; } = null!;

    public static CertificateLevelDTO CertificateLevelToDto(CertificateLevel certificateLevel)
    {
        return new CertificateLevelDTO { Id = certificateLevel.Id, Level = certificateLevel.Level };
    }
}