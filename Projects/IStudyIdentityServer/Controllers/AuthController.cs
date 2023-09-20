using System.Text.Json;
using System.Text.Json.Nodes;
using Firebase.Auth;
using IStudyIdentityServer.Data;
using IStudyIdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User = IStudyIdentityServer.Data.User;

namespace IStudyIdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly FirebaseAuthProvider _auth;
    private readonly IConfiguration _configuration;
    private readonly IStudyDataBaseContext _context;
    private readonly UserTokenStorageContext _tokenContext;

    public AuthController(IConfiguration config, IStudyDataBaseContext context, UserTokenStorageContext tokenContext)
    {
        _configuration = config;
        _auth = new FirebaseAuthProvider(
            new FirebaseConfig(_configuration.GetValue<string>("FirebaseKey")));
        _context = context;
        _tokenContext = tokenContext;
    }

    [Route("SignIn")]
    [HttpPost]
    public async Task<IActionResult> SignIn(UserLoginModel model)
    {
        try
        {
            var fbUserLink = await _auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);

            var token = fbUserLink.FirebaseToken;

            _tokenContext.UserTokens.Add(
                new UserToken { AccessToken = token, RefreshToken = fbUserLink.RefreshToken });
            await _tokenContext.SaveChangesAsync();

            return token == null ? Problem("Can not get token") : Ok(new { access_token = token });
        }
        catch (FirebaseAuthException ex)
        {
            var firebaseEx = JsonSerializer.Deserialize<FirebaseError>(ex.ResponseData);
            return Problem(firebaseEx.error.message);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    public record RefreshTokenModel(string AccessToken);
    
    [Route("RefreshToken")]
    [HttpPost]
    public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
    {
        try
        {
            var userToken = await _tokenContext.UserTokens.FirstOrDefaultAsync(x => x.AccessToken == model.AccessToken);
            if (userToken == null)
                return BadRequest();

            var postContentRaw = new
            {
                grant_type = "refresh_token",
                refresh_token = userToken.RefreshToken
            };

            var postContent = new StringContent(JsonSerializer.Serialize(postContentRaw));
            postContent.Headers.Clear();
            postContent.Headers.Add("Content-Type", "application/json");

            var response = await WebClient.Client.PostAsync(
                $"https://securetoken.googleapis.com/v1/token?key={_configuration.GetValue<string>("FirebaseKey")}",
                postContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonObject.Parse(await response.Content.ReadAsStreamAsync());
                var new_access_token = responseData["access_token"].Deserialize<string>();
                var new_refresh_token = responseData["refresh_token"].Deserialize<string>();
                userToken.AccessToken = new_access_token;
                userToken.RefreshToken = new_refresh_token;
                await _tokenContext.SaveChangesAsync();
                return Ok(new { access_token = new_access_token});
            }

            return Unauthorized();

        }
        catch (FirebaseAuthException ex)
        {
            var firebaseEx = JsonSerializer.Deserialize<FirebaseError>(ex.ResponseData);
            return Problem(firebaseEx.error.message);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Route("SignUp")]
    [HttpPost]
    public async Task<IActionResult> SignUp(UserRegistrationModel model)
    {
        try
        {
            var fbUserLink = await _auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password);

            var newUser = new User
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

            _tokenContext.UserTokens.Add(new UserToken { AccessToken = token, RefreshToken = fbUserLink.RefreshToken });
            await _tokenContext.SaveChangesAsync();

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