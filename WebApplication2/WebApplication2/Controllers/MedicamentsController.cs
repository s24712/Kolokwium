using WebApplication2.Models;

namespace WebApplication2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class MedicamentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MedicamentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Medicament>> GetMedicament(int id)
    {
        var medicament = await _context.Medicament.FindAsync(id);
        if (medicament == null)
        {
            return NotFound();
        }

        var prescriptions = await _context.PrescriptionMedicament
            .Where(pm => pm.IdMedicament == id)
            .Select(pm => new { pm.Prescription.Date, pm.Details })
            .OrderByDescending(pm => pm.Date)
            .ToListAsync();
        return Ok(new { Medicament = medicament, Prescriptions = prescriptions });
    }
}