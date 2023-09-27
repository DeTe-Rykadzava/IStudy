using Microsoft.Build.Framework;

namespace IStudyAPI.Data;

public class UserPhotoModel
{
    [Required] public string FileB64Data { get; set; }
}