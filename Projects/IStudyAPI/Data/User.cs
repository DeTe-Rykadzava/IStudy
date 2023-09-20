using System;
using System.Collections.Generic;

namespace IStudyAPI.Data;

public partial class User
{
    public Guid Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string? Lastname { get; set; }

    public int Verifystage { get; set; }

    public int UserTypeId { get; set; }

    public string UserPhoto { get; set; } = null!;

    public int ClassId { get; set; }

    public string Userid { get; set; } = null!;

    public virtual ICollection<Certificate> CertificateAddedUsers { get; set; } = new List<Certificate>();

    public virtual ICollection<CertificateOwner> CertificateOwners { get; set; } = new List<CertificateOwner>();

    public virtual ICollection<Certificate> CertificateTeacherUsers { get; set; } = new List<Certificate>();

    public virtual Class Class { get; set; } = null!;

    public virtual UserType UserType { get; set; } = null!;
}
