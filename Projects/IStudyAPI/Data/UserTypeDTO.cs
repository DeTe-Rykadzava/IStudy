namespace IStudyAPI.Data;

public class UserTypeDTO
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public static UserTypeDTO UserTypeToDto(UserType userType)
    {
        return new UserTypeDTO { Id = userType.Id, Type = userType.Type };
    }
}