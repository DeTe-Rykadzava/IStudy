namespace IStudyIdentityServer.Data;

public class UserToken
{
    public int Id { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}