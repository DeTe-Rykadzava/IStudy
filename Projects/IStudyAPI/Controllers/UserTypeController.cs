using IStudyAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IStudyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserTypeController : ControllerBase
{
    private readonly IstudyDataBaseContext _context;

    public UserTypeController(IstudyDataBaseContext context)
    {
        _context = context;
    }

    // GET: api/UserType
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserTypeDTO>>> GetUserTypes()
    {
        return await _context.UserTypes.Where(x => x.Id != 3)
            .Select(s => UserTypeToDto(s))
            .ToListAsync();
    }

    // PUT: api/UserType/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserType(int id, UserTypeDTO userTypeDto)
    {
        if (!(await BaseController.CanUserModifiedEntry(_context, HttpContext)))
        {
            return Forbid();
        }

        if (id != userTypeDto.Id) return BadRequest();

        var userType = await _context.UserTypes.FirstOrDefaultAsync(x => x.Id == userTypeDto.Id);

        if (userType == null)
            return BadRequest();

        userType.Type = userTypeDto.Type;

        _context.Entry(userType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

        return NoContent();
    }

    // POST: api/UserType
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<UserTypeDTO>> PostUserType(UserType userType)
    {
        if (!(await BaseController.CanUserModifiedEntry(_context, HttpContext)))
        {
            return Forbid();
        }
        
        _context.UserTypes.Add(userType);
        await _context.SaveChangesAsync();

        return UserTypeToDto(userType);
    }

    [Authorize]
    // DELETE: api/UserType/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserType(int id)
    {
        if (!(await BaseController.CanUserModifiedEntry(_context, HttpContext)))
        {
            return Forbid();
        }

        var userType = await _context.UserTypes.FindAsync(id);
        if (userType == null) return NotFound();

        _context.UserTypes.Remove(userType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserTypeExists(int id)
    {
        return (_context.UserTypes?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    private static UserTypeDTO UserTypeToDto(UserType userType)
    {
        return new UserTypeDTO() { Id = userType.Id, Type = userType.Type };
    }
}