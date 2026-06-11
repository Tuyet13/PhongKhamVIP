using Microsoft.AspNetCore.Mvc;

namespace PhongKhamVIP.Controllers.Clinical
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
