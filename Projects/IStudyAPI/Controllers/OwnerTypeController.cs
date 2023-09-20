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
    public class OwnerTypeController : ControllerBase
    {
        private readonly IstudyDataBaseContext _context;

        public OwnerTypeController(IstudyDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/OwnerType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificateOwnerType>>> GetCertificateOwnerTypes()
        {
            return await _context.CertificateOwnerTypes.ToListAsync();
        }

        // PUT: api/OwnerType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificateOwnerType(int id, CertificateOwnerTypeDTO certificateOwnerTypeDto)
        {
            if (!(await BaseController.CanUserModifiedEntry(_context, HttpContext)))
            {
                return Forbid();
            }
            
            if (id != certificateOwnerTypeDto.Id)
            {
                return BadRequest();
            }

            var certificateOwnerType =
                await _context.CertificateOwnerTypes.FirstOrDefaultAsync(x => x.Id == certificateOwnerTypeDto.Id);

            if (certificateOwnerType == null)
                return BadRequest();

            certificateOwnerType.OwnerType = certificateOwnerTypeDto.OwnerType;
    
            _context.Entry(certificateOwnerType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateOwnerTypeExists(id))
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

        // POST: api/OwnerType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CertificateOwnerTypeDTO>> PostCertificateOwnerType(CertificateOwnerType certificateOwnerType)
        {
            if (!(await BaseController.CanUserModifiedEntry(_context, HttpContext)))
            {
                return Forbid();
            }
            
            _context.CertificateOwnerTypes.Add(certificateOwnerType);
            await _context.SaveChangesAsync();

            return CertificateOwnerTypeToDto(certificateOwnerType);
        }

        // DELETE: api/OwnerType/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificateOwnerType(int id)
        {
            if (!(await BaseController.CanUserModifiedEntry(_context, HttpContext)))
            {
                return Forbid();
            }
            
            var certificateOwnerType = await _context.CertificateOwnerTypes.FindAsync(id);
            if (certificateOwnerType == null)
            {
                return NotFound();
            }

            _context.CertificateOwnerTypes.Remove(certificateOwnerType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CertificateOwnerTypeExists(int id)
        {
            return (_context.CertificateOwnerTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static CertificateOwnerTypeDTO CertificateOwnerTypeToDto(CertificateOwnerType ownerType)
        {
            return new CertificateOwnerTypeDTO { Id = ownerType.Id, OwnerType = ownerType.OwnerType };
        }
    }
}
