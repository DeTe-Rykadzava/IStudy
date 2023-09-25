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
    public class ClassController : BaseController
    {
        private readonly IstudyDataBaseContext _context;

        public ClassController(IstudyDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Class
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassDTO>>> GetClasses()
        {
            return await _context.Classes.Select(s => ClassToDto(s)).ToListAsync();
        }

        // PUT: api/Class/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClass(int id, ClassDTO classDto)
        {
            if (!(await IsAdmin(_context, HttpContext)))
            {
                return Forbid();
            }
            
            if (id != classDto.Id)
            {
                return BadRequest();
            }

            var @class = await _context.Classes.FirstOrDefaultAsync(x => x.Id == classDto.Id);

            if (@class == null)
                return BadRequest();

            @class.Name = classDto.Name;

            _context.Entry(@class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
                {
                    return NotFound();
                }

                return Problem();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return NoContent();
        }

        // POST: api/Class
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ClassDTO>> PostClass(ClassDTO @class)
        {
            if (!(await IsAdmin(_context, HttpContext)))
            {
                return Forbid();
            }

            var newClass = new Class
            {
                Name = @class.Name
            };
            
            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return ClassToDto(newClass);
        }

        // DELETE: api/Class/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (!(await IsAdmin(_context, HttpContext)))
            {
                return Forbid();
            }
            
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(@class);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClassExists(int id)
        {
            return (_context.Classes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static ClassDTO ClassToDto(Class @class)
        {
            return new ClassDTO { Id = @class.Id, Name = @class.Name };
        }
    }
}
