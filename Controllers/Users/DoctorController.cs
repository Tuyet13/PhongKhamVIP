using Microsoft.AspNetCore.Mvc;

namespace PhongKhamVIP.Controllers.Users
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
