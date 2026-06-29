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

        // 1. Danh sách bệnh nhân
        public async Task<IActionResult> Index(string searchString)
        {
            var patients = _context.Patients.Include(p => p.User).AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p =>
                    p.FullName.Contains(searchString) ||
                    (p.Phone != null && p.Phone.Contains(searchString)));
            }

            ViewBag.SearchString = searchString;
            return View("/Views/Admin/Patient/Index.cshtml", await patients.ToListAsync());
        }

        // 2. Chi tiết
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null) return NotFound();

            return View("/Views/Admin/Patient/Details.cshtml", patient);
        }

        // 3. Edit (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            return View("/Views/Admin/Patient/Edit.cshtml", patient);
        }

        // 4. Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Đính kèm và cập nhật những thay đổi
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin bệnh nhân thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể lưu thay đổi. Vui lòng kiểm tra lại dữ liệu.");
                }
            }
            return View("/Views/Admin/Patient/Edit.cshtml", patient);
        }

        // 5. Xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bệnh nhân khỏi hệ thống!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}