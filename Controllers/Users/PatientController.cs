using Microsoft.AspNetCore.Mvc;

namespace PhongKhamVIP.Controllers.Users
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
