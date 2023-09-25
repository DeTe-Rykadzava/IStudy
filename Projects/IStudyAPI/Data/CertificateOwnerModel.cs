namespace IStudyAPI.Data;

public class CertificateOwnerModel
{
    public UserDTO OwnerUser { get; set; }
    
    public int CertificateOwnerTypeId { get; set; }
}