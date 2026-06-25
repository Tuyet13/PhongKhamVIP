using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data; // Đảm bảo namespace này đúng với dự án của bạn
using System.Linq;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers.Doctor
{
    public class UserMedicineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserMedicineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action: GET /Medicine/Index
        public async Task<IActionResult> Index(string searchString)
        {
            var medicines = _context.Medicines.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                medicines = medicines.Where(m => m.Name.Contains(searchString));
            }

            // ĐƯỜNG DẪN CHÍNH XÁC:
            return View("~/Views/Doctor/UserMedicine/MedicineList.cshtml", await medicines.ToListAsync());
        }
    }
}