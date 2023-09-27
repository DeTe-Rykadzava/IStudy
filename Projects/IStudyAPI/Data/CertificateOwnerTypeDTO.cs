namespace IStudyAPI.Data;

public class CertificateOwnerTypeDTO
{
    public int Id { get; set; }

    public string OwnerType { get; set; } = null!;
    
    public static CertificateOwnerTypeDTO CertificateOwnerTypeToDto(CertificateOwnerType ownerType)
    {
        return new CertificateOwnerTypeDTO { Id = ownerType.Id, OwnerType = ownerType.OwnerType };
    }
}