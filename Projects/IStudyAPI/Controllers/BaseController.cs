using IStudyAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IStudyAPI.Controllers;

public class BaseController : ControllerBase
{
    protected async Task<bool> IsAdmin(IstudyDataBaseContext context)
    {
        var currentUserId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
        
        var user = await context.Users.FirstOrDefaultAsync(x => 
            x.Id == currentUserId);

        if (user == null)
            return false;
        if (user.UserTypeId != 3)
            return false;
        return true;
    }

    protected async Task<IActionResult?> IsUser(IstudyDataBaseContext context)
    {
        var currentUserId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
        
        var user = await context.Users.FirstOrDefaultAsync(x => 
            x.Id == currentUserId);

        if (user == null)
            return Unauthorized();
        if (user.UserTypeId == 3)
            return Forbid();
        return null;
    }
}