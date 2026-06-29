using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.Users; // Đảm bảo namespace này đúng với nơi bạn để model
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhongKhamVIP.Controllers
{
    [Authorize(Roles = "Patient")]
    public class AccountProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Profile/MyProfile
        public async Task<IActionResult> MyProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            return View(patient); // Trỏ tới Views/PatientDashboard/Profile/MyProfile.cshtml
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            return View(patient); // Trỏ tới Views/PatientDashboard/Profile/Edit.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyProfile");
            }
            return View(patient);
        }
    }
}