using Microsoft.AspNetCore.Mvc;

namespace PhongKhamVIP.Controllers.Clinical
{
    public class MedicalRecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
