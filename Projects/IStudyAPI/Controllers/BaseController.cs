using IStudyAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IStudyAPI.Controllers;

public static class BaseController
{
    public static async Task<bool> CanUserModifiedEntry(IstudyDataBaseContext context, HttpContext httpContext)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == httpContext.User.FindFirst("user_id").Value);

        if (user == null)
            return false;
        if (user.UserTypeId != 3)
            return false;
        return true;
    }
}