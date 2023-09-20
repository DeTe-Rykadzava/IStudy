using System.ComponentModel.DataAnnotations;

namespace IStudyIdentityServer.Models;

public class UserRegistrationModel
{
    [Required] public string Firstname { get; set; } = null!;

    [Required] public string Secondname { get; set; } = null!;

    public string? Lastname { get; set; }

    public int? Classid { get; set; }

    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;

    [Required] [Range(1, 2)] public int Usertypeid { get; set; }
}