using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKhamVaDatLich.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GioiThieu()
        {
            return View();
        }

        public IActionResult DichVu()
        {
            return View();
        }

        public IActionResult NhaThuoc()
        {
            return View();
        }

        public IActionResult LienHe()
        {
            return View();
        }
    }
}