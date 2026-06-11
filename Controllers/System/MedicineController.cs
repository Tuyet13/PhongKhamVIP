using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Clinical;

namespace PhongKhamVIP.Controllers.System
{
    public class MedicineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DANH SÁCH + TÌM KIẾM
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Medicines.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(m =>
                    m.Name.Contains(searchString) ||
                    m.Unit.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            var medicines = await query
                .OrderBy(m => m.Name)
                .ToListAsync();

            return View("~/Views/Admin/Medicine/Index.cshtml", medicines);
        }

        // CHI TIẾT
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine == null)
                return NotFound();

            return View("~/Views/Admin/Medicine/Details.cshtml", medicine);
        }

        // FORM THÊM
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Medicine/Create.cshtml");
        }

        // THÊM THUỐC
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Medicine/Create.cshtml", medicine);

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm thuốc thành công!";

            return RedirectToAction(nameof(Index));
        }

        // FORM SỬA
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine == null)
                return NotFound();

            return View("~/Views/Admin/Medicine/Edit.cshtml", medicine);
        }

        // LƯU SỬA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine)
        {
            if (id != medicine.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Medicine/Edit.cshtml", medicine);

            _context.Update(medicine);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thuốc thành công!";

            return RedirectToAction(nameof(Index));
        }

        // FORM XÓA
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine == null)
                return NotFound();

            return View("~/Views/Admin/Medicine/Delete.cshtml", medicine);
        }

        // XÁC NHẬN XÓA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine == null)
                return NotFound();

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa thuốc thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}