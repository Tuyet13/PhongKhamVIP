using Microsoft.AspNetCore.Mvc;
using PhongKhamVIP.Data;
using System.Linq;

namespace PhongKhamVIP.Controllers
{
    public class ManageAppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageAppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Đổi tên Action này thành ManageAppointments
        public IActionResult ManageAppointments()
        {
            // Chỉ định đường dẫn View cụ thể nằm trong thư mục Receptionist
            return View("~/Views/Receptionist/ManageAppointments.cshtml");
        }
    }
}