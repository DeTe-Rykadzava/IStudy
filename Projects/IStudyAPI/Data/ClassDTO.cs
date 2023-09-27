namespace IStudyAPI.Data;

public class ClassDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public static ClassDTO ClassToDto(Class @class)
    {
        return new ClassDTO { Id = @class.Id, Name = @class.Name };
    }
}