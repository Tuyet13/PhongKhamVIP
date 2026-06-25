using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers
{
    [Authorize(Roles = "Receptionist, Admin")]
    public class ReceptionistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReceptionistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Dashboard - Trang chủ Lễ tân
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard Lễ Tân";
            return View();
        }

        // 2. Danh sách lịch hẹn
        public async Task<IActionResult> ManageAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
            return View(appointments); 
        }

        // 3. Action Check-in (POST) - Cập nhật trạng thái
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                TempData["Error"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction(nameof(ManageAppointments));
            }

            // Cập nhật trạng thái
            appointment.Status = "CheckedIn";

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã tiếp nhận bệnh nhân thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật trạng thái.";
            }

            return RedirectToAction(nameof(ManageAppointments));
        }


       
    }
}