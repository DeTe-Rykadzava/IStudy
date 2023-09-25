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
    public class CertificateController : BaseController
    {
        private readonly IstudyDataBaseContext _context;

        public CertificateController(IstudyDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Certificate
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        { 
            return await _context.Certificates.ToListAsync();
        }

        // GET: api/Certificate/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return certificate;
        }

        // PUT: api/Certificate/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificate(int id, Certificate certificate)
        {
            if (id != certificate.Id)
            {
                return BadRequest();
            }

            _context.Entry(certificate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateExists(id))
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

        // POST: api/Certificate
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CertificateDTO>> PostCertificate(CertificateModel model)
        {
            var currentUserId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
        
            var user = await _context.Users.FirstOrDefaultAsync(x => 
                x.Id == currentUserId);

            if (user == null)
                return Unauthorized();
            
            var newCertificate = new Certificate
            {
                AddedUserId = user.Id,
                CertificateLevelId = model.CertificateLevelId,
                Createdate = DateTime.Now,
                Title = model.Title,
            };
            
            
            
            _context.Certificates.Add(newCertificate);
            await _context.SaveChangesAsync();

            return CertificateToDto(newCertificate);
        }

        // DELETE: api/Certificate/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            if (!(await IsAdmin(_context, HttpContext)))
                return Forbid();
            
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CertificateExists(int id)
        {
            return (_context.Certificates?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static CertificateDTO CertificateToDto(Certificate certificate)
        {
            return new CertificateDTO
            {
                Id = certificate.Id,
                CertificateLevel = certificate.CertificateLevel.Level,
                CreateDate = certificate.Createdate,
                Title = certificate.Title,
                AddedUser = $"{certificate.AddedUser.Firstname} {certificate.AddedUser.Secondname} {certificate.AddedUser.Lastname}"
            };
        }
    }
}
