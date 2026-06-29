using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Users; // Đảm bảo đúng namespace của Patient
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers.Patients
{
    [Authorize(Roles = "Patient")]
    public class PatientProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var patient = await _context.Patients
                .Include(p => p.User) // Load thông tin user đi kèm
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null) return NotFound();

            // Chỉ định rõ đường dẫn View
            return View("~/Views/PatientDashboard/Profile/MyProfile.cshtml", patient);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return NotFound();

            return View("~/Views/PatientDashboard/Profile/Edit.cshtml", patient);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (!ModelState.IsValid) return View("~/Views/PatientDashboard/PatientProfile/Edit.cshtml", patient);

            try
            {
                _context.Update(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật hồ sơ thành công!";
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Không thể lưu thay đổi.");
                return View("~/Views/PatientDashboard/PatientProfile/Edit.cshtml", patient);
            }

            return RedirectToAction("Index");
        }
    }
}