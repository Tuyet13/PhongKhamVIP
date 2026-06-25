using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers.System
{
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AuditLog (Danh sách lịch sử thao tác)
        public async Task<IActionResult> Index(string searchUser, string searchType)
        {
            // Lấy dữ liệu từ bảng AuditLogs, xếp nhật ký mới nhất lên đầu trang
            var logs = _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .AsQueryable();

            // Tìm kiếm theo tên tài khoản thực hiện (ActionBy)
            if (!string.IsNullOrEmpty(searchUser))
            {
                logs = logs.Where(l => l.ActionBy.Contains(searchUser));
            }

            // Lọc theo phân loại hành động (ActionType)
            if (!string.IsNullOrEmpty(searchType))
            {
                logs = logs.Where(l => l.ActionType.Contains(searchType));
            }

            ViewBag.SearchUser = searchUser;
            ViewBag.SearchType = searchType;

            // Render dữ liệu ra file View Index
            return View("~/Views/Admin/AuditLog/Index.cshtml", await logs.ToListAsync());
        }

        // GET: AuditLog/Details/5 (Xem chi tiết nội dung thay đổi)
        public async Task<IActionResult> Details(int id)
        {
            var log = await _context.AuditLogs.FirstOrDefaultAsync(l => l.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/AuditLog/Details.cshtml", log);
        }
    }
}