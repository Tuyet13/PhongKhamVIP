using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PhongKhamVIP.Models.Clinical;

namespace PhongKhamVIP.Controllers.Receptionist
{
    [Authorize(Roles = "Receptionist, Admin")]
    public class ReceptionistAppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReceptionistAppointmentController(ApplicationDbContext context) => _context = context;

        // Danh sách lịch hẹn: /ReceptionistAppointment/Index
        public async Task<IActionResult> Index()
        {
            var list = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return View("~/Views/Receptionist/Appointment/Index.cshtml", list);
        }

        // Tạo lịch khám mới: /ReceptionistAppointment/Create
        public IActionResult Create()
        {
            return View("~/Views/Receptionist/Appointment/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (ModelState.IsValid)
            {
                _context.Appointments.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Receptionist/Appointment/Create.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái
            appointment.Status = "CheckedIn"; // Hoặc trạng thái bạn mong muốn
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}