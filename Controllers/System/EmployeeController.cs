using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Users;

namespace PhongKhamVIP.Controllers.System
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // DANH SÁCH + TÌM KIẾM
        // ==========================
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Users
                .Where(u => u.Role == "Doctor" || u.Role == "Receptionist");

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(u =>
                    u.FullName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.Phone.Contains(searchString) ||
                    u.Username.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            var staffList = await query
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View("~/Views/Admin/Employee/Index.cshtml", staffList);
        }

        // ==========================
        // CHI TIẾT
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Users.FindAsync(id);

            if (employee == null)
                return NotFound();

            return View("~/Views/Admin/Employee/Details.cshtml", employee);
        }

        // ==========================
        // FORM THÊM
        // ==========================
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Employee/Create.cshtml");
        }

        // ==========================
        // THÊM NHÂN VIÊN
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User employee)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Employee/Create.cshtml", employee);

            employee.CreatedAt = DateTime.Now;

            _context.Users.Add(employee);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm nhân viên thành công!";

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // FORM SỬA
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Users.FindAsync(id);

            if (employee == null)
                return NotFound();

            return View("~/Views/Admin/Employee/Edit.cshtml", employee);
        }

        // ==========================
        // LƯU SỬA
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User employee)
        {
            if (id != employee.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Employee/Edit.cshtml", employee);

            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật nhân viên thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == employee.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // FORM XÓA
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Users.FindAsync(id);

            if (employee == null)
                return NotFound();

            return View("~/Views/Admin/Employee/Delete.cshtml", employee);
        }

        // ==========================
        // XÁC NHẬN XÓA
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Users.FindAsync(id);

            if (employee == null)
                return NotFound();

            _context.Users.Remove(employee);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa nhân viên thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}