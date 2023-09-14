using System.ComponentModel.DataAnnotations;

namespace IStudyIdentityServer.Models;

public class UserLoginModel
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    
    public string Password { get; set; }
}