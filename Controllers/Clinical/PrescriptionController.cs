using Microsoft.AspNetCore.Mvc;

namespace PhongKhamVIP.Controllers.Clinical
{
    public class PrescriptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
