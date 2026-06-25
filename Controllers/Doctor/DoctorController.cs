using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace PhongKhamVIP.Controllers.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId.ToString() == doctorId)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
            return View(appointments);
        }

        public async Task<IActionResult> MedicalExamination(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);
            return appointment == null ? NotFound() : View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var app = await _context.Appointments.FindAsync(id);
            if (app != null)
            {
                app.Status = newStatus;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}