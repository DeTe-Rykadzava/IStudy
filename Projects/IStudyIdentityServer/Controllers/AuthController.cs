using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Firebase.Auth;
using IStudyIdentityServer.Data;
using IStudyIdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IStudyIdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FirebaseAuthProvider _auth;
        private readonly IStudyDataBaseContext _context;

        public AuthController(IConfiguration config, IStudyDataBaseContext context)
        {
            _auth = new FirebaseAuthProvider(
                new FirebaseConfig(config.GetValue<string>("FirebaseKey")));
            _context = context;
        }

        [Route("SignIn")]
        [HttpPost]
        public async Task<IActionResult> SignIn(UserLoginModel model)
        {
            try
            {
                var fbuserLink = await _auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);

                var token = fbuserLink.FirebaseToken;

                return token == null ? Problem("Can not get token") : Ok(new { access_token = token });
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonSerializer.Deserialize<FirebaseError>(ex.ResponseData);
                return Problem(firebaseEx.error.message);
            }
        }
        
        [Route("SignUp")]
        [HttpPost]
        public async Task<IActionResult> SignUp(UserRegistrationModel model)
        {
            try
            {
                var fbUserLink = await _auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password);

                var newUser = new Data.User()
                {
                    Id = fbUserLink.User.LocalId,
                    Firstname = model.Firstname,
                    Secondname = model.Secondname,
                    Lastname = model.Lastname,
                    VerifyStage = 0,
                    ClassId = model.Classid,
                    UserTypeId = model.Usertypeid
                };

                _context.Users.Add(newUser);

                await _context.SaveChangesAsync();

                var token = fbUserLink.FirebaseToken;

                return token == null ? Problem("Can not get token") : Ok(new { access_token = token });
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonSerializer.Deserialize<FirebaseError>(ex.ResponseData);
                return BadRequest(firebaseEx.error.message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        
    }
}
