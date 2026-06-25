using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PhongKhamVIP.Controllers
{
    [Authorize] // Khớp với giá trị "Patient" trong DB
    public class PatientDashboardController : Controller
    {
        // Action chính để kiểm tra truy cập
        public IActionResult Index()
        {
            // Nếu bạn gặp lỗi 403, hãy mở comment dòng dưới để debug:
            // var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Content("Đã đăng nhập thành công! Role nhận được: " + User.FindFirst(ClaimTypes.Role)?.Value);
        }

        public IActionResult CreateAppointment() => View();
        public IActionResult MyAppointments() => View();
        public IActionResult MyInvoices() => View();
        public IActionResult MyPrescriptions() => View();
    }
}