namespace IStudyAPI.Data;

public class UserDTO
{
    public string Id { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string? Lastname { get; set; }

    public string UserType { get; set; } = null!;

    public string Class { get; set; } = null!;
}