using IStudyAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IStudyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CertificateLevelController : BaseController
{
    private readonly IstudyDataBaseContext _context;

    public CertificateLevelController(IstudyDataBaseContext context)
    {
        _context = context;
    }

    // GET: api/CertificateLevel
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CertificateLevelDTO>>> GetCertificateLevels()
    {
        return await _context.CertificateLevels
            .Select(s => CertificateLevelDTO.CertificateLevelToDto(s))
            .ToListAsync();
    }

    // PUT: api/CertificateLevel/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCertificateLevel(int id, CertificateLevelDTO certificateLevel)
    {
        if (!await IsAdmin(_context)) return Forbid();

        if (id != certificateLevel.Id) return BadRequest();

        _context.Entry(certificateLevel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CertificateLevelExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/CertificateLevel
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CertificateLevelDTO>> PostCertificateLevel(CertificateLevelDTO certificateLevel)
    {
        if (!await IsAdmin(_context)) return Forbid();

        var newCertificateLevel = new CertificateLevel
        {
            Id = certificateLevel.Id,
            Level = certificateLevel.Level
        };

        _context.CertificateLevels.Add(newCertificateLevel);
        await _context.SaveChangesAsync();

        return CertificateLevelDTO.CertificateLevelToDto(newCertificateLevel);
    }

    // DELETE: api/CertificateLevel/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCertificateLevel(int id)
    {
        if (!await IsAdmin(_context)) return Forbid();

        var certificateLevel = await _context.CertificateLevels.FindAsync(id);
        if (certificateLevel == null) return NotFound();

        _context.CertificateLevels.Remove(certificateLevel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CertificateLevelExists(int id)
    {
        return (_context.CertificateLevels?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}