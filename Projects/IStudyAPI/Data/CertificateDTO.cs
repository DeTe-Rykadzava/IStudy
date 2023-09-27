namespace IStudyAPI.Data;

public class CertificateDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string AddedUser { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string CertificateLevel { get; set; }

    public List<CertificateOwnerDTO> Owners { get; set; }

    public static CertificateDTO CertificateToDto(Certificate certificate)
    {
        return new CertificateDTO
        {
            Id = certificate.Id,
            CertificateLevel = certificate.CertificateLevel.Level,
            CreateDate = certificate.Createdate,
            Title = certificate.Title,
            AddedUser =
                $"{certificate.AddedUser.Firstname} {certificate.AddedUser.Secondname} {certificate.AddedUser.Lastname}",
            Owners = certificate.CertificateOwners
                .Select(s => new CertificateOwnerDTO
                {
                    Owner = UserDTO.UserToDto(s.CertificateOwnerUser),
                    OwnerType = CertificateOwnerTypeDTO.CertificateOwnerTypeToDto(s.CertificateOwnerTypeNavigation)
                })
                .ToList()
        };
    }
}