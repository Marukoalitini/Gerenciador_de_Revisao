using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motos.Data;
using Motos.Models;

namespace Motos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MotosController : ControllerBase
{
    private readonly AppDbContext _context;

    public MotosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Motos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Moto>>> GetMotos()
    {
        return await _context.Motos.ToListAsync();
    }

    // GET: api/Motos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Moto>> GetMoto(int id)
    {
        var moto = await _context.Motos.FindAsync(id);

        if (moto == null)
        {
            return NotFound();
        }

        return moto;
    }

    // POST: api/Motos
    [HttpPost]
    public async Task<ActionResult<Moto>> PostMoto(Moto moto)
    {
        _context.Motos.Add(moto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, moto);
    }

    // PUT: api/Motos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMoto(int id, Moto moto)
    {
        if (id != moto.Id)
        {
            return BadRequest();
        }

        _context.Entry(moto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MotoExists(id))
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

    // DELETE: api/Motos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMoto(int id)
    {
        var moto = await _context.Motos.FindAsync(id);
        if (moto == null)
        {
            return NotFound();
        }

        _context.Motos.Remove(moto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MotoExists(int id)
    {
        return _context.Motos.Any(e => e.Id == id);
    }
}
