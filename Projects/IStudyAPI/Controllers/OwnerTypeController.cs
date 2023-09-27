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
    public class OwnerTypeController : BaseController
    {
        private readonly IstudyDataBaseContext _context;

        public OwnerTypeController(IstudyDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/OwnerType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificateOwnerTypeDTO>>> GetCertificateOwnerTypes()
        {
            return await _context.CertificateOwnerTypes
                .Select(s => CertificateOwnerTypeDTO.CertificateOwnerTypeToDto(s))
                .ToListAsync();
        }

        // PUT: api/OwnerType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutCertificateOwnerType(CertificateOwnerTypeDTO certificateOwnerTypeDto)
        {
            if (!(await IsAdmin(_context)))
            {
                return Forbid();
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
        public async Task<ActionResult<CertificateOwnerTypeDTO>> PostCertificateOwnerType(CertificateOwnerTypeDTO certificateOwnerType)
        {
            if (!(await IsAdmin(_context)))
            {
                return Forbid();
            }

            var newOwnerType = new CertificateOwnerType
            {
                OwnerType = certificateOwnerType.OwnerType
            };
            
            _context.CertificateOwnerTypes.Add(newOwnerType);
            await _context.SaveChangesAsync();

            return CertificateOwnerTypeDTO.CertificateOwnerTypeToDto(newOwnerType);
        }

        // DELETE: api/OwnerType/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificateOwnerType(int id)
        {
            if (!(await IsAdmin(_context)))
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
    }
}
