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
        public async Task<ActionResult<UserDTO>> GetUserInfo()
        {
            var user = await _context.Users
                .Include(i => i.Class)
                .Include(i => i.UserType)
                .FirstOrDefaultAsync(x => x.Id == User.FindFirst("user_id").Value);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDto(user);
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, UserDTO userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userDto.Id);

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
                if (!UserExists(id))
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserPhoto()
        {
            var userId = User.FindFirst("user_id").Value;
            
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return BadRequest();
            
           _context.Entry(user).State = EntityState.Modified;

            try
            {
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
                Class = user.Class.Name,
                UserType = user.UserType.Type
            };
        }
    }
}
