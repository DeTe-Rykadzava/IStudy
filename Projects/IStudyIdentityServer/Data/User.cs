using System;
using System.Collections.Generic;

namespace IStudyIdentityServer.Data;

public partial class User
{
    public string Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string? Lastname { get; set; }

    public int VerifyStage { get; set; }

    public int UserTypeId { get; set; }

    public string UserPhoto { get; set; } = null!;

    public int? ClassId { get; set; }
}
