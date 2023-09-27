using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IStudyAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public async Task<ActionResult<IEnumerable<CertificateDTO>>> GetCertificates()
        { 
            if((await IsAdmin(_context)))
            {
                return await _context.Certificates
                        .Include(i => i.CertificateOwners)
                        .ThenInclude(i => i.CertificateOwnerUser)
                        .Include(i => i.CertificateOwners)
                        .ThenInclude(i => i.CertificateOwnerTypeNavigation)
                        .Include(i => i.CertificateLevel)
                        .Include(i => i.AddedUser)
                        .Select(s => CertificateDTO.CertificateToDto(s))
                        .ToListAsync();
            }
            else
            {
                return BadRequest();
            }
        }

        // GET: api/Certificate/5
        [Authorize]
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<CertificateDTO>>> GetCertificatesByUserId(string userId)
        {
            return await _context.Certificates
                .Include(i => i.CertificateOwners)
                .ThenInclude(i => i.CertificateOwnerUser)
                .Include(i => i.CertificateOwners)
                .ThenInclude(i => i.CertificateOwnerTypeNavigation)
                .Include(i => i.CertificateLevel)
                .Include(i => i.AddedUser)
                .Where(x => x.AddedUserId == userId)
                .Select(s => CertificateDTO.CertificateToDto(s))
                .ToListAsync();
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutCertificateFile(CertificatePutFileModel model)
        {
            var currentUserId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
                
            var user = await _context.Users.FirstOrDefaultAsync(x => 
                x.Id == currentUserId);
                
            if (user == null)
                return Unauthorized();

            var certificate = await _context.Certificates.FirstOrDefaultAsync(x => x.Id == model.CertificateId);

            if (certificate == null)
                return BadRequest();

            if (certificate.AddedUserId != user.Id)
            {
                if (!await IsAdmin(_context))
                    return Forbid();
            }

            if (string.IsNullOrWhiteSpace(model.FileB64))
                return BadRequest();
            
            var path = $"{Directory.GetCurrentDirectory()}\\Certificates";
                
            Directory.CreateDirectory(path);
    
            using (var fs = new FileStream(certificate.FilePath, FileMode.Create))
            {
                var fileBytes = Convert.FromBase64String(model.FileB64);
                        
                var ms = new MemoryStream(fileBytes);
                await ms.CopyToAsync(fs);
                await ms.DisposeAsync();
            }

            return NoContent();
        }

        // PUT: api/Certificate/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutCertificate(CertificateDTO certificateDto)
        {
            try
            {
                var currentUserId = User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;
                
                var user = await _context.Users.FirstOrDefaultAsync(x => 
                    x.Id == currentUserId);
                
                if (user == null)
                    return Unauthorized();
                
                var certificate = await _context.Certificates
                    .Include(i => i.CertificateOwners)
                    .ThenInclude(i => i.CertificateOwnerUser)
                    .Include(i => i.CertificateOwners)
                    .ThenInclude(i => i.CertificateOwnerTypeNavigation)
                    .Include(i => i.CertificateLevel)
                    .Include(i => i.AddedUser)
                    .FirstOrDefaultAsync(x => x.Id == certificateDto.Id);
                
                if (certificate == null)
                    return BadRequest();
                
                if (certificate.AddedUserId != user.Id)
                {
                    if(!await IsAdmin(_context))
                        return Forbid();
                }

                
                certificate.Title = certificate.Title != certificateDto.Title 
                                    ? certificateDto.Title 
                                    : certificate.Title;

                var certificateLevel =
                    await _context.CertificateLevels.FirstOrDefaultAsync(
                        x => x.Level == certificateDto.CertificateLevel);

                certificate.CertificateLevelId = certificate.CertificateLevelId != certificateLevel!.Id
                                                 ? certificateLevel.Id
                                                 : certificate.CertificateLevelId;
                // adding/edit owners
                foreach (var owner in certificateDto.Owners)
                {
                    var ownerCertificate = certificate.CertificateOwners.FirstOrDefault(x => x.CertificateOwnerUser.Id == owner.Owner.Id);
                    // add
                    if (ownerCertificate == null)
                    {
                        await _context.CertificateOwners.AddAsync(new CertificateOwner()
                        {
                            CertificateId = certificate.Id,
                            CertificateOwnerUserId = owner.Owner.Id,
                            CertificateOwnerType = owner.OwnerType.Id
                        });
                    }
                    else
                    {
                        ownerCertificate.CertificateOwnerType = ownerCertificate.CertificateOwnerType != owner.OwnerType.Id
                                                                ? owner.OwnerType.Id
                                                                : ownerCertificate.CertificateOwnerType;
                    }
                }
                
                //remove
                foreach (var certificateOwner in certificate.CertificateOwners)
                {
                    var owner = certificateDto.Owners.FirstOrDefault(x =>
                        x.Owner.Id == certificateOwner.CertificateOwnerUserId);
                    if (owner == null)
                    {
                        _context.CertificateOwners.Remove(certificateOwner);
                    }
                }
                
                _context.Entry(certificate).State = EntityState.Modified;
            
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
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

            try
            {
                var newCertificate = new Certificate
                {
                    AddedUserId = user.Id,
                    CertificateLevelId = model.CertificateLevelId,
                    Createdate = DateTime.Now,
                    Title = model.Title,
                };
                
                var path = $"{Directory.GetCurrentDirectory()}\\Certificates";
                
                var fileName = $"[{DateTime.Today}]_{model.Title}.pdf";
                
                Directory.CreateDirectory(path);
    
                using (var fs = new FileStream(Path.Combine(path,fileName), FileMode.Create))
                {
                    var fileBytes = Convert.FromBase64String(model.FileB64);
                        
                    var ms = new MemoryStream(fileBytes);
                    await ms.CopyToAsync(fs);
                    await ms.DisposeAsync();
                }

                newCertificate.FilePath = Path.Combine(path, fileName);
                
                _context.Certificates.Add(newCertificate);
                await _context.SaveChangesAsync();

                foreach (var owner in model.Owners)
                {
                    _context.CertificateOwners.Add(new CertificateOwner()
                    {
                        CertificateId = newCertificate.Id,
                        CertificateOwnerUserId = owner.OwnerUserId,
                        CertificateOwnerType = owner.CertificateOwnerTypeId
                    });
                    
                    await _context.SaveChangesAsync();
                }

                var currentCertificate =
                    await _context.Certificates
                        .Include(i => i.CertificateOwners)
                        .ThenInclude(i => i.CertificateOwnerUser)
                        .Include(i => i.CertificateOwners)
                        .ThenInclude(i => i.CertificateOwnerTypeNavigation)
                        .Include(i => i.CertificateLevel)
                        .Include(i => i.AddedUser)
                        .FirstOrDefaultAsync(x => x.Id == newCertificate.Id);
                
                return CertificateDTO.CertificateToDto(currentCertificate!);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Problem(e.Message);
            }
        }

        // DELETE: api/Certificate/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            if (!(await IsAdmin(_context)))
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
    }
}
