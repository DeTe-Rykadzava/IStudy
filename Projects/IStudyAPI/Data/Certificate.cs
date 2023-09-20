using System;
using System.Collections.Generic;

namespace IStudyAPI.Data;

public partial class Certificate
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly CreateDate { get; set; }

    public string Filename { get; set; } = null!;

    public Guid? TeacherUserId { get; set; }

    public Guid AddedUserId { get; set; }

    public virtual User AddedUser { get; set; } = null!;

    public virtual ICollection<CertificateOwner> CertificateOwners { get; set; } = new List<CertificateOwner>();

    public virtual User? TeacherUser { get; set; }
}
