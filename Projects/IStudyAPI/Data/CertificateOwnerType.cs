using System;
using System.Collections.Generic;

namespace IStudyAPI.Data;

public partial class CertificateOwnerType
{
    public int Id { get; set; }

    public string OwnerType { get; set; } = null!;

    public virtual ICollection<CertificateOwner> CertificateOwners { get; set; } = new List<CertificateOwner>();
}
