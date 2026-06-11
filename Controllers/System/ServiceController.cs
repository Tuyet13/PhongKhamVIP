using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Finance;

namespace PhongKhamVIP.Controllers.System
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // DANH SÁCH + TÌM KIẾM
        // ==========================
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Services.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    s.Name.Contains(searchString) ||
                    s.Description.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            var services = await query
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View("~/Views/Admin/Service/Index.cshtml", services);
        }

        // ==========================
        // CHI TIẾT
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            return View("~/Views/Admin/Service/Details.cshtml", service);
        }

        // ==========================
        // FORM THÊM
        // ==========================
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Service/Create.cshtml");
        }

        // ==========================
        // THÊM DỊCH VỤ
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Service/Create.cshtml", service);

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm dịch vụ thành công!";

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // FORM SỬA
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            return View("~/Views/Admin/Service/Edit.cshtml", service);
        }

        // ==========================
        // LƯU SỬA
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Service/Edit.cshtml", service);

            _context.Update(service);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật dịch vụ thành công!";

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // FORM XÓA
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            return View("~/Views/Admin/Service/Delete.cshtml", service);
        }

        // ==========================
        // XÁC NHẬN XÓA
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa dịch vụ thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}