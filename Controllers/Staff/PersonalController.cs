using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Finance;
using PhongKhamVIP.Models.System;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers.Staff
{
    public class PersonalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonalController(ApplicationDbContext context) => _context = context;

        // Hàm hỗ trợ tự động chọn Layout theo Role
        private string GetUserLayout()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role == "Doctor" ? "~/Views/Shared/_LayoutDoctor.cshtml" : "~/Views/Shared/_ReceptionistLayout.cshtml";
        }

        // --- 1. CHẤM CÔNG ---
        public async Task<IActionResult> Attendance()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Auth");

            ViewBag.Layout = GetUserLayout();
            var userId = int.Parse(userIdStr);
            var data = await _context.Attendances
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View("~/Views/Staff/Personal/Attendance.cshtml", data);
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdStr);
            var today = DateTime.Now.Date;
            bool alreadyChecked = await _context.Attendances.AnyAsync(a => a.UserId == userId && a.Date == today);

            if (!alreadyChecked)
            {
                _context.Attendances.Add(new Attendance { UserId = userId, Date = today, CheckIn = DateTime.Now.TimeOfDay, Status = "Checked-In" });
                await _context.SaveChangesAsync();
                TempData["Message"] = "Đã chấm công!";
            }
            else TempData["Error"] = "Bạn đã chấm công rồi!";

            return RedirectToAction("Attendance");
        }

        // --- 2. THÔNG BÁO ---
        public async Task<IActionResult> Notifications()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Layout = GetUserLayout();
            var userId = int.Parse(userIdStr);
            var data = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View("~/Views/Staff/Personal/Notifications.cshtml", data);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
        // --- 3. NGHỈ PHÉP ---
        public IActionResult LeaveRequest()
        {
            ViewBag.Layout = GetUserLayout();
            return View("~/Views/Staff/Personal/LeaveRequest.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LeaveRequest(DateTime StartDate, DateTime EndDate, string Reason)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            _context.LeaveRequests.Add(new LeaveRequest { UserId = userId, StartDate = StartDate, EndDate = EndDate, Reason = Reason, Status = "Pending", CreatedAt = DateTime.Now });
            await _context.SaveChangesAsync();
            TempData["Message"] = "Đã gửi đơn!";
            return RedirectToAction("LeaveRequest");
        }

        // --- 4. LƯƠNG ---
        public async Task<IActionResult> Salary()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Layout = GetUserLayout();
            var userId = int.Parse(userIdStr);
            var salary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Month == DateTime.Now.Month && s.Year == DateTime.Now.Year);

            return View("~/Views/Staff/Personal/Salary.cshtml", salary);
        }
    }
}