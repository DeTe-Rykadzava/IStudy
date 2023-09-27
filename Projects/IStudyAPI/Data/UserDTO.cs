namespace IStudyAPI.Data;

public class UserDTO
{
    public string Id { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string? Lastname { get; set; }

    public string UserType { get; set; } = null!;

    public string? Class { get; set; }

    public static UserDTO UserToDto(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Secondname = user.Secondname,
            Lastname = user.Lastname,
            Class = user.Class?.Name,
            UserType = user.UserType.Type
        };
    }
}