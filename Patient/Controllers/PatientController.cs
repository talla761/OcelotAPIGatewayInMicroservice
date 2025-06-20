using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patient.Data;
using Patient.Models;
using System.Threading.Tasks;

namespace Patient.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientController> _logger;

        public PatientController(ApplicationDbContext context, ILogger<PatientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/patient
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _context.Patients.ToListAsync();
            return Ok(patients);
        }

        // GET: api/patient/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        // POST: api/patient
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient.Models.Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = patient.PatientId }, patient);
        }

        // PUT: api/patient/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient.Models.Patient updatedPatient)
        {
            if (id != updatedPatient.PatientId)
                return BadRequest("ID mismatch");

            if (!await _context.Patients.AnyAsync(p => p.PatientId == id))
                return NotFound();

            _context.Entry(updatedPatient).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/patient/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
