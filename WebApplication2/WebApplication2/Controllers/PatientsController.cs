namespace WebApplication2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            var prescriptions = await _context.Prescription
                .Where(p => p.IdPatient == id)
                .ToListAsync();
            foreach (var prescription in prescriptions)
            {
                var prescriptionMedicament = await _context.PrescriptionMedicament
                    .Where(pm => pm.IdPrescription == prescription.IdPrescription)
                    .ToListAsync();
                _context.PrescriptionMedicament.RemoveRange(prescriptionMedicament);
            }

            _context.Prescription.RemoveRange(prescriptions);
            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return NoContent();
        }
        catch (System.Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Internal Server Error: " + ex.Message);
        }
    }
}