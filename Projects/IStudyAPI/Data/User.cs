namespace IStudyAPI.Data;

public class User
{
    public string Id { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string? Lastname { get; set; }

    public int Verifystage { get; set; }

    public int UserTypeId { get; set; }

    public string UserPhoto { get; set; } = null!;

    public int ClassId { get; set; }

    public virtual ICollection<CertificateOwner> CertificateOwners { get; set; } = new List<CertificateOwner>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual Class Class { get; set; } = null!;

    public virtual UserType UserType { get; set; } = null!;
}