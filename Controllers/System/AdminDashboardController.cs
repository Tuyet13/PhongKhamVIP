using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PhongKhamVIP.Controllers.System
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalPatients = await _context.Users.CountAsync(u => u.Role == "Patient");
            ViewBag.PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending");
            ViewBag.TodayRevenue = (await _context.Invoices.SumAsync(i => i.TotalAmount)).ToString("N0") + " VND";

            // Lấy danh sách dùng .Select trực tiếp
            ViewBag.RecentAppointments = await _context.Appointments
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .Select(a => new {
                    PatientName = a.Patient.FullName,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status
                }).ToListAsync();
            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
        
    }
}