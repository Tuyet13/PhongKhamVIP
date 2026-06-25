using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Finance;
using PhongKhamVIP.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // =========================================================================
        // TÍNH NĂNG CHẤM CÔNG REALTIME - NHẬT KÝ HỆ THỐNG (ĐỒNG BỘ SANG INT USERID)
        // =========================================================================

        // GET: /Employee/AttendanceLog
        public async Task<IActionResult> AttendanceLog(DateTime? targetDate)
        {
            DateTime today = targetDate ?? DateTime.Today;
            ViewBag.SelectedDate = today.ToString("yyyy-MM-dd");

            var users = await _context.Users
                .Where(u => u.Role == "Doctor" || u.Role == "Receptionist")
                .ToListAsync();

            // Đã đồng bộ: ToDictionary với Key là int (a.UserId)
            var attendances = await _context.Attendances
                .Where(a => a.Date == today)
                .ToDictionaryAsync(a => a.UserId);

            ViewBag.Attendances = attendances;

            return View("~/Views/Admin/Employee/AttendanceLog.cshtml", users);
        }

        // POST: /Employee/UpdateAttendanceQuick
        [HttpPost]
        public async Task<IActionResult> UpdateAttendanceQuick(int userId, string date, string status, string? checkIn, string? checkOut, string? note)
        {
            if (userId <= 0)
            {
                return Json(new { success = false, message = "Mã nhân viên không hợp lệ!" });
            }

            DateTime parsedDate = DateTime.Parse(date);

            // So sánh kiểu int == int cực kỳ chuẩn xác
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == parsedDate);

            if (attendance == null)
            {
                attendance = new Attendance
                {
                    UserId = userId,
                    Date = parsedDate,
                    Status = status,
                    Note = note
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                attendance.Status = status;
                attendance.Note = note;
            }

            if (!string.IsNullOrEmpty(checkIn)) attendance.CheckIn = TimeSpan.Parse(checkIn);
            else attendance.CheckIn = null;

            if (!string.IsNullOrEmpty(checkOut)) attendance.CheckOut = TimeSpan.Parse(checkOut);
            else attendance.CheckOut = null;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật nhật ký chấm công thành công!" });
        }

        // GET: /Employee/StaffCheckIn (Màn hình máy chấm công nhân viên tự bấm)
        public async Task<IActionResult> StaffCheckIn()
        {
            // Thay vì chuỗi string, ép thành int để lấy thông tin tài khoản hiện tại từ Session/Cookie của bạn
            // Giả định Id người dùng đang đăng nhập là 1 (Hãy thay bằng biến lấy từ Identity/Session thực tế của bạn)
            int currentUserId = 1;
            DateTime today = DateTime.Today;

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == currentUserId && a.Date == today);

            return View("~/Views/Admin/Employee/StaffCheckIn.cshtml", attendance);
        }

        // POST: /Employee/ProcessCheckInOrOut
        [HttpPost]
        public async Task<IActionResult> ProcessCheckInOrOut()
        {
            // Đồng bộ sang dạng kiểu int
            int currentUserId = 1;
            DateTime today = DateTime.Today;
            TimeSpan nowTime = DateTime.Now.TimeOfDay;

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == currentUserId && a.Date == today);

            if (attendance == null)
            {
                attendance = new Attendance
                {
                    UserId = currentUserId,
                    Date = today,
                    CheckIn = nowTime,
                    Status = nowTime > new TimeSpan(8, 0, 0) ? "Late" : "Present",
                    Note = "Máy tự động Check-In"
                };
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                return Json(new { success = true, type = "In", time = nowTime.ToString(@"hh\:mm"), status = attendance.Status });
            }
            else if (attendance.CheckOut == null)
            {
                attendance.CheckOut = nowTime;
                attendance.Note += " & Máy tự động Check-Out";
                _context.Update(attendance);
                await _context.SaveChangesAsync();
                return Json(new { success = true, type = "Out", time = nowTime.ToString(@"hh\:mm") });
            }

            return Json(new { success = false, message = "Nhân viên này đã hoàn thành chấm công hôm nay!" });
        }
    }
}