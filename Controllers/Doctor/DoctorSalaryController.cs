using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace PhongKhamVIP.Controllers.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class DoctorSalaryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorSalaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action Index để hiển thị danh sách lương của bác sĩ
        public async Task<IActionResult> Index()
        {
            // Lấy ID của bác sĩ từ Claims sau khi đăng nhập
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // Lấy toàn bộ lịch sử lương của bác sĩ đó, sắp xếp theo thời gian mới nhất
            var salaryList = await _context.Salaries
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .ToListAsync();

            return View(salaryList);
        }
    }
}