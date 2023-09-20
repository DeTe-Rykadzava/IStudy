using System;
using System.Collections.Generic;

namespace IStudyAPI.Data;

public partial class CertificateOwner
{
    public int Id { get; set; }

    public Guid UserOwnerId { get; set; }

    public int CertificateId { get; set; }

    public virtual Certificate Certificate { get; set; } = null!;

    public virtual User UserOwner { get; set; } = null!;
}
