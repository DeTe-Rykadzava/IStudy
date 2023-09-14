using System;
using System.Collections.Generic;

namespace IStudyIdentityServer.Data;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string? Lastname { get; set; }

    public int Classid { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Verifystage { get; set; }

    public int Usertypeid { get; set; }
}
