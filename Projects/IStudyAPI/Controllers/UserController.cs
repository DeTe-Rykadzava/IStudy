using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IStudyAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace IStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IstudyDataBaseContext _context;

        public UserController(IstudyDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/User
        [Authorize]
        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        { 
            return await _context.Users
                .Include(i => i.UserType)
                .Include(i => i.Class)
                .Where(x => x.UserType.Id != 3)
                .Select(s => UserToDto(s))
                .ToListAsync();
        }

        // GET: api/User/5
        [Authorize]
        [HttpGet]
        [Route("UserInfo")]
        public async Task<ActionResult<UserDTO>> GetUserInfo()
        {
            // Console.WriteLine(User.Claims.FirstOrDefault(x => x.Type == "user_id").Value);

            var userId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
            
            var user = await _context.Users
                .Include(i => i.Class)
                .Include(i => i.UserType)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDto(user);
        }

        [Authorize]
        [HttpGet]
        [Route("UserPhoto/{userId}")]
        public async Task<IActionResult> GetUserPhoto(string userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                
                if (user == null)
                    return NotFound();

                if (string.IsNullOrWhiteSpace(user.UserPhoto))
                    return NotFound();

                var photoBytes = System.IO.File.ReadAllBytes(user.UserPhoto);
                
                return File(photoBytes, "image/jpg");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Problem(e.Message);
            }
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(UserDTO userDto)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
            
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return BadRequest();
            
            user.Firstname = userDto.Firstname;
            user.Secondname = userDto.Secondname;
            user.Lastname = userDto.Lastname;
            user.Class = (await _context.Classes.FirstOrDefaultAsync(x => x.Name == userDto.Class))!;
            
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutUserPhoto(UserPhotoModel model)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
                
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
    
                if (user == null)
                    return BadRequest();

                if (string.IsNullOrWhiteSpace(model.FileB64Data))
                    return BadRequest();
    
                var path = $"{Directory.GetCurrentDirectory()}\\Users_Photos";
    
                Directory.CreateDirectory(path);
    
                using (var sw = new FileStream(Path.Combine(path,$"{userId}.jpg"), FileMode.Create))
                {
                    var photoBytes = Convert.FromBase64String(model.FileB64Data);
                    
                    var ms = new MemoryStream(photoBytes);
                    await ms.CopyToAsync(sw);
                    await ms.DisposeAsync();
                }
    
                user.UserPhoto = Path.Combine(path, $"{userId}.jpg");
                
                _context.Entry(user).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return NoContent();
        }
        
        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static UserDTO UserToDto(User user)
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
}
