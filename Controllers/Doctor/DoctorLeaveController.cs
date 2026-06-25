using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.System;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class DoctorLeaveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorLeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn nghỉ của bác sĩ
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var leaves = await _context.LeaveRequests
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
            return View(leaves);
        }

        // Hiển thị form tạo đơn
        [HttpGet]
        public IActionResult Create() => View();

        // Xử lý lưu đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequest model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                model.Status = "Pending"; // Mặc định là chờ duyệt

                _context.LeaveRequests.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}