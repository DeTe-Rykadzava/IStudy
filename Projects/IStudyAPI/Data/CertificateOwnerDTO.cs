namespace IStudyAPI.Data;

public class CertificateOwnerDTO
{
    public UserDTO Owner { get; set; }
    
    public CertificateOwnerTypeDTO OwnerType { get; set; }
}