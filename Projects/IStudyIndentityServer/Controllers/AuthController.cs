using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IStudyIndentityServer.Data;
using IStudyIndentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IStudyIndentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IStudyContext _context;

        public AuthController(IStudyContext context) => _context = context;

        [Route("SignIn")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] UserLoginModel model)
        {

            var user = await AuthenticateUser(model.Login, model.Password);
            if (user == null)
                return Unauthorized();
            
            return Ok();
        }
        
        [Route("SignUp")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody]UserRegistrationModel model)
        {
            if (await UserExistByLogin(model.Login))
                return Conflict("User with that login already exist");
            
            return Ok();
        }
        
        [Route("CheckCanLogin")]
        [HttpPost]
        public async Task<bool> CheckCanLogin([FromBody] string login)
        {
            if (await UserExistByLogin(login))
                return true;
            return false;
        }

        private async Task<User?> AuthenticateUser(string login, string password )
        {
           return (await GetAll()).FirstOrDefault(x => x.Login == login && BCrypt.Net.BCrypt.Verify(password, x.Password));
        }

        private async Task<List<User>> GetAll()
        {
            return await _context.Users.Where(x => x.Verifystage == 1).ToListAsync();
        }

        private async Task<bool> UserExistByLogin (string login)
        {
            return (await GetAll()).FirstOrDefault(x => x.Login == login) != null;
        }
    }
}
