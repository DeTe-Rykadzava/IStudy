using System;
using System.Collections.Generic;

namespace IStudyAPI.Data;

public partial class Certificate
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string AddedUserId { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime Createdate { get; set; }

    public int CertificateLevelId { get; set; }

    public virtual User AddedUser { get; set; } = null!;

    public virtual CertificateLevel CertificateLevel { get; set; } = null!;

    public virtual ICollection<CertificateOwner> CertificateOwners { get; set; } = new List<CertificateOwner>();
}
