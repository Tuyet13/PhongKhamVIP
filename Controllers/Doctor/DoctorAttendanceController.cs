using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Finance; // Namespace chứa model Attendance
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class DoctorAttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorAttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang hiển thị lịch sử chấm công
        public async Task<IActionResult> Index()
        {
            // Lấy ID người dùng từ Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var attendanceList = await _context.Attendances
                .Where(a => a.UserId == userId) // Dùng UserId theo model mới
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(attendanceList);
        }

        // Xử lý Check-in
        [HttpPost]
        public async Task<IActionResult> CheckIn()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaim);

            var attendance = new Attendance
            {
                UserId = userId,
                Date = DateTime.Now.Date,
                CheckIn = DateTime.Now.TimeOfDay, // Dùng TimeSpan theo model mới
                Status = "Present"
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Đã Check-in thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Xử lý Check-out
        [HttpPost]
        public async Task<IActionResult> CheckOut(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                attendance.CheckOut = DateTime.Now.TimeOfDay; // Dùng TimeSpan theo model mới
                await _context.SaveChangesAsync();
                TempData["Message"] = "Đã Check-out thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}