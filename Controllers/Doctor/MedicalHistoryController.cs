using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PhongKhamVIP.Controllers.Doctor
{
    public class MedicalHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action: Hiển thị danh sách lịch sử khám bệnh
        public async Task<IActionResult> Index()
        {
            // Lấy ID người dùng đang đăng nhập (ví dụ: PatientId)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            int patientId = int.Parse(userIdClaim);

            // Truy vấn lịch sử khám bệnh của bệnh nhân này
            var history = await _context.MedicalRecords
                .Include(m => m.Doctor) // Lấy thông tin bác sĩ
                .ThenInclude(d => d.User) // Lấy tên bác sĩ từ bảng User
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return View("~/Views/Doctor/MedicalRecord/Index.cshtml", history);
        }
    }
}