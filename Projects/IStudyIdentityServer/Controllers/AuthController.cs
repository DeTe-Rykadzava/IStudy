using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IStudyIdentityServer.Common;
using IStudyIdentityServer.Data;
using IStudyIdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace IStudyIdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IStudyContext _context;
        private readonly IOptions<AuthOptions> _authOptions;
        public AuthController(IStudyContext context, IOptions<AuthOptions> authOptions) => (_context, _authOptions) = (context, authOptions);

        [Route("SignIn")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] UserLoginModel model)
        {
            var user = await AuthenticateUser(model.Login, model.Password);
            if (user == null)
                return Unauthorized();

            var token = GenerateJwt(user);
            
            return Ok(new {access_token = token});
        }
        
        [Route("SignUp")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody]UserRegistrationModel model)
        {
            if (!(await UserExistByLogin(model.Login)))
                return Conflict("User with that login already exist");
            try
            {
                var newUser = new User()
                {
                    Classid = model.Classid,
                    Firstname = model.Firstname,
                    Id = Guid.NewGuid(),
                    Verifystage = 0,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Usertypeid = model.Usertypeid,
                    Login = model.Login,
                    Secondname = model.Secondname,
                    Lastname = model.Lastname
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                
                var token = GenerateJwt(newUser);
            
                return Ok(new {access_token = token});
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
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

        private string GenerateJwt(User user)
        {
            var authParams = _authOptions.Value;
            var securityKey = authParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email,user.Login),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };
            
            switch (user.Usertypeid)
            {
                case 1:
                    claims.Add(new Claim("role","student"));
                    break;
                case 2:
                    claims.Add(new Claim("role", "teacher"));
                    break;
            }

            var token = new JwtSecurityToken(
                authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifetime),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
