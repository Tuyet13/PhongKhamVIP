using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PhongKhamVIP.Controllers.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class PrescriptionController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Kê đơn thuốc";
            // Trỏ về view chung trong thư mục Doctor
            return View("~/Views/Doctor/Prescription.cshtml");
        }
    }
}