using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PhongKhamVIP.Models.Finance;
using PhongKhamVIP.Models.System;

namespace PhongKhamVIP.Controllers.System
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalPatients = await _context.Patients.CountAsync();
            ViewBag.PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending" || a.Status == "Chờ khám");
            var revenue = await _context.Invoices.Where(i => i.Status == "Paid" || i.Status == "Đã thanh toán").SumAsync(i => (decimal?)i.TotalAmount) ?? 0;
            ViewBag.TodayRevenue = revenue.ToString("N0") + " VND";
            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }

        // Quản lý đơn nghỉ phép
        [HttpGet]
        public async Task<IActionResult> ManageLeaveRequests()
        {
            var requests = await _context.LeaveRequests
                .Include(r => r.User)
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View("~/Views/Admin/Leave/ManageLeaveRequests.cshtml", requests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLeaveStatus(int id, string status)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = status;

                // Gửi thông báo cho nhân viên
                var notify = new Notification
                {
                    UserId = request.UserId,
                    Message = $"Đơn xin nghỉ phép ngày {request.StartDate.ToShortDateString()} của bạn đã được {status}.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(notify);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Đã cập nhật trạng thái đơn thành công!";
            }
            return RedirectToAction("ManageLeaveRequests");
        }
    }
}