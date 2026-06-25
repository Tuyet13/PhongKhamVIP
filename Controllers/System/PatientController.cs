using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Users;
using System.Linq;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers.System
{
    [Authorize(Roles = "Admin")]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            // Lấy dữ liệu từ DB, bao gồm thông tin User (nếu có)
            var patients = _context.Patients.Include(p => p.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p =>
                    p.FullName.Contains(searchString) ||
                    (p.Phone != null && p.Phone.Contains(searchString)));
            }

            ViewBag.SearchString = searchString;
            return View("/Views/Admin/Patient/Index.cshtml", await patients.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null) return NotFound();

            return View("/Views/Admin/Patient/Details.cshtml", patient);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View("/Views/Admin/Patient/Edit.cshtml", patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id) return NotFound();

            // Loại bỏ User khỏi validate vì User là quan hệ khóa ngoại
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                _context.Update(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("/Views/Admin/Patient/Edit.cshtml", patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bệnh nhân!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}