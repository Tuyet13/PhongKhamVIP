using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Clinical;
using PhongKhamVIP.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PhongKhamVIP.Models.Finance;
using PhongKhamVIP.Models.System;
namespace PhongKhamVIP.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // URL: /PatientDashboard/Index
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var viewModel = new PatientDashboardViewModel
            {
                UpcomingAppointment = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Where(a => a.Patient.UserId == userId && a.AppointmentDate > DateTime.Now)
                    .OrderBy(a => a.AppointmentDate)
                    .FirstOrDefaultAsync(),
                RecentAppointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Where(a => a.Patient.UserId == userId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(5)
                    .ToListAsync(),
                Notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync()
            };
            return View("~/Views/PatientDashboard/Index.cshtml", viewModel);
        }

        // URL: /PatientDashboard/MyAppointments
        public async Task<IActionResult> MyAppointments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var list = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.Patient.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
            return View("~/Views/PatientDashboard/MyAppointment.cshtml", list);
        }

        // URL: /PatientDashboard/MyPrescriptions
        public async Task<IActionResult> MyPrescriptions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var records = await _context.MedicalRecords
                .Include(mr => mr.Prescriptions)
                .Where(mr => mr.Patient.UserId == userId)
                .ToListAsync();
            return View("~/Views/PatientDashboard/MyPrescriptions.cshtml", records);
        }

        // URL: /PatientDashboard/MyInvoices
        public async Task<IActionResult> MyInvoices()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var invoices = await _context.Invoices
                .Where(i => i.Patient.UserId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            // Trỏ đúng về file MyInvoice.cshtml
            return View("~/Views/PatientDashboard/MyInvoice.cshtml", invoices);
        }

        // URL: /PatientDashboard/BookAppointment
        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            var model = new BookAppointmentViewModel
            {
                Doctors = await _context.Doctors
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName + " - " + d.Specialty.Name })
                    .ToListAsync()
            };
            return View("~/Views/PatientDashboard/CreateAppointment.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

                if (patient == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin bệnh nhân.");
                }
                else
                {
                    var appointment = new Appointment
                    {
                        PatientId = patient.Id,
                        DoctorId = model.DoctorId,
                        AppointmentDate = model.AppointmentDate,
                        Status = "Pending",
                        Notes = model.Note
                    };

                    _context.Appointments.Add(appointment);

                    // --- ĐOẠN CODE GỬI THÔNG BÁO CHO RECEPTIONIST ---
                    var receptionistRole = "Receptionist";
                    var receptionists = await _context.Users
                        .Where(u => u.Role == receptionistRole)
                        .ToListAsync();

                    foreach (var admin in receptionists)
                    {
                        var notification = new Notification
                        {
                            UserId = admin.Id,
                            Message = $"Bệnh nhân {User.Identity.Name} vừa đặt lịch hẹn mới vào ngày {model.AppointmentDate:dd/MM/yyyy HH:mm}.",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };
                        _context.Notifications.Add(notification);
                    }
                    // ------------------------------------------------

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            // Nạp lại danh sách bác sĩ nếu model không hợp lệ
            model.Doctors = await _context.Doctors
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FullName + " - " + d.Specialty.Name
                })
                .ToListAsync();

            return View("~/Views/PatientDashboard/CreateAppointment.cshtml", model);
        }
    }
}