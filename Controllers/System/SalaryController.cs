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
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                salary.NetSalary = salary.BaseSalary + salary.Bonus - salary.Deductions;

                _context.Salaries.Add(salary);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm bảng lương thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = await _context.Users
                .Where(u => u.Role == "Doctor" || u.Role == "Receptionist")
                .ToListAsync();

            return View("~/Views/Admin/Salary/Create.cshtml", salary);
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

        // API AJAX: Xác nhận tích nhanh đổi trạng thái bảng lương
        [HttpPost]
        public async Task<IActionResult> UpdateStatusQuick(int id, string status = "Paid")
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bảng lương yêu cầu!" });
            }

            salary.Status = status;

            if (status == "Paid")
            {
                salary.PaidDate = DateTime.Now;
            }

            _context.Update(salary);
            await _context.SaveChangesAsync();

            return Json(new { success = true, currentStatus = status, paidDate = salary.PaidDate?.ToString("dd/MM/yyyy") });
        }

        // API AJAX: Thống kê ngày công
       
        [HttpGet]
        public async Task<IActionResult> GetEmployeeWorkSummary(int userId, int month, int year)
        {
            if (userId == 0 || month == 0 || year == 0)
                return Json(new { workingDays = 0, absentDays = 0 });

            // 1. Số ngày làm việc thực tế
            var workingDays = await _context.Attendances
                .CountAsync(a => a.UserId == userId &&
                                 a.Date.Month == month &&
                                 a.Date.Year == year &&
                                 a.Status == "Checked-In");

            // 2. Tính số ngày nghỉ (Absent)
            // Cách 1: Đếm số bản ghi có Status == "Absent" trong DB
            var absentDays = await _context.Attendances
                .CountAsync(a => a.UserId == userId &&
                                 a.Date.Month == month &&
                                 a.Date.Year == year &&
                                 a.Status == "Absent");

            return Json(new { workingDays = workingDays, absentDays = absentDays });
        }
    }
}