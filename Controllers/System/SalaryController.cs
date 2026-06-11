using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Finance;

namespace PhongKhamVIP.Controllers.System
{
    public class SalaryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salary
        public async Task<IActionResult> Index(string searchString)
        {
            var salaries = _context.Salaries
                .Include(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                salaries = salaries.Where(s =>
                    s.User.FullName.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            return View("~/Views/Admin/Salary/Index.cshtml", await salaries.ToListAsync());
        }

        // GET: Salary/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var salary = await _context.Salaries
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salary == null)
                return NotFound();

            return View("~/Views/Admin/Salary/Details.cshtml", salary);
        }

        // GET: Salary/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users
                .Where(u => u.Role == "Doctor" || u.Role == "Receptionist")
                .ToListAsync();

            return View("~/Views/Admin/Salary/Create.cshtml");
        }

        // POST: Salary/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salary salary)
        {
            // Bắt buộc loại bỏ kiểm tra thuộc tính liên kết để ModelState không bị False vô lý
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                salary.NetSalary = salary.BaseSalary + salary.Bonus - salary.Deductions;

                _context.Salaries.Add(salary);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm bảng lương thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi Validate, nạp lại danh sách nhân viên trước khi trả về View
            ViewBag.Users = await _context.Users
                .Where(u => u.Role == "Doctor" || u.Role == "Receptionist")
                .ToListAsync();

            return View("~/Views/Admin/Salary/Create.cshtml", salary);
        }

        // GET: Salary/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);

            if (salary == null)
                return NotFound();

            ViewBag.Users = await _context.Users
                .Where(u => u.Role == "Doctor" || u.Role == "Receptionist")
                .ToListAsync();

            return View("~/Views/Admin/Salary/Edit.cshtml", salary);
        }

        // POST: Salary/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Salary salary)
        {
            if (id != salary.Id)
                return NotFound();

            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                salary.NetSalary = salary.BaseSalary + salary.Bonus - salary.Deductions;

                _context.Update(salary);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật bảng lương thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = await _context.Users
                .Where(u => u.Role == "Doctor" || u.Role == "Receptionist")
                .ToListAsync();

            return View("~/Views/Admin/Salary/Edit.cshtml", salary);
        }

        // GET: Salary/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var salary = await _context.Salaries
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salary == null)
                return NotFound();

            return View("~/Views/Admin/Salary/Delete.cshtml", salary);
        }

        // POST: Salary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);

            if (salary != null)
            {
                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Xóa bảng lương thành công!";
            return RedirectToAction(nameof(Index));
        }

        // API AJAX: Tính số ngày công dựa trên Nhân viên, Tháng và Năm
        [HttpGet]
        public async Task<IActionResult> GetEmployeeWorkSummary(string userId, int month, int year)
        {
            if (string.IsNullOrEmpty(userId) || month == 0 || year == 0)
                return Json(new { workingDays = 0, absentDays = 0 });

            // Đếm số ngày đi làm (Present)
            var workingDays = await _context.Attendances
                .CountAsync(a => a.UserId == userId &&
                                 a.Date.Month == month &&
                                 a.Date.Year == year &&
                                 a.Status == "Present");

            // Đếm số ngày nghỉ (Absent)
            var absentDays = await _context.Attendances
                .CountAsync(a => a.UserId == userId &&
                                 a.Date.Month == month &&
                                 a.Date.Year == year &&
                                 a.Status == "Absent");

            return Json(new { workingDays = workingDays, absentDays = absentDays });
        }
    }
}