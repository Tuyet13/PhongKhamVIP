using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
using PhongKhamVIP.Models.System;
using PhongKhamVIP.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// Các thư viện xử lý Đăng nhập hệ thống bằng Cookie sinh thẻ bài (Claims)
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

// Thư viện MailKit xử lý hạ tầng gửi Mail thật qua Google SMTP
using MimeKit;
using MailKit.Security;

namespace PhongKhamVIP.Controllers.Users
{
    // Class nhận dữ liệu đăng nhập từ giao diện View
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. CHỨC NĂNG ĐĂNG NHẬP TRUYỀN THỐNG (PASSWORD LOGIN FLOW)
        // =========================================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Auth/Login.cshtml");
            }

            // Tìm kiếm người dùng dựa trên Email hoặc Username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Email);

            if (user != null && user.PasswordHash == model.Password)
            {
                // Vì IsActive được gắn [NotMapped], ta xử lý kiểm tra trạng thái hoạt động trực tiếp tại đây nếu cần thiết
                await SignInUserAsync(user);
                return RedirectUserByRole(user.Role);
            }

            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác.");
            return View("~/Views/Auth/Login.cshtml");
        }

        // =========================================================================
        // 2. CHỨC NĂNG ĐĂNG KÝ TÀI KHOẢN BỆNH NHÂN (REGISTER FLOW)
        // =========================================================================

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string phoneNumber, string username, string password)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Để đảm bảo quyền lợi, Quý khách vui lòng điền đầy đủ tất cả các trường thông tin được yêu cầu.");
                return View("~/Views/Auth/Register.cshtml");
            }

            bool isUsernameExist = await _context.Users.AnyAsync(u => u.Username == username);
            if (isUsernameExist)
            {
                ModelState.AddModelError("", "Tên đăng nhập này đã tồn tại trên hệ thống. Quý khách vui lòng lựa chọn tên khác.");
                return View("~/Views/Auth/Register.cshtml");
            }

            bool isEmailExist = await _context.Users.AnyAsync(u => u.Email == email);
            if (isEmailExist)
            {
                ModelState.AddModelError("", "Địa chỉ Email này đã được đăng ký thành viên.");
                return View("~/Views/Auth/Register.cshtml");
            }

            try
            {
                string randomOtp = new Random().Next(100000, 999999).ToString();

                var otpEntry = new OtpVerification
                {
                    Receiver = email, // FIX TỪ 'Target' THÀNH 'Receiver' ĐỂ ĐỒNG BỘ MODEL VÀ DB
                    OtpCode = randomOtp,
                    ExpiredAt = DateTime.Now.AddMinutes(5),
                    IsUsed = false
                };
                _context.OtpVerifications.Add(otpEntry);
                await _context.SaveChangesAsync();

                await SendOtpEmailReal(email, randomOtp);

                TempData["Reg_FullName"] = fullName;
                TempData["Reg_Email"] = email;
                TempData["Reg_Phone"] = phoneNumber;
                TempData["Reg_Username"] = username;
                TempData["Reg_Password"] = password;
                TempData["IsRegisterFlow"] = true;
                TempData["UserTarget"] = email;

                return RedirectToAction("VerifyOtp");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Không thể gửi mã xác thực vào thời điểm này: {ex.Message}. Vui lòng thử lại sau.");
                return View("~/Views/Auth/Register.cshtml");
            }
        }

        // =========================================================================
        // 3. TRANG XÁC THỰC MÃ OTP VÀ ĐĂNG NHẬP NHANH BẰNG OTP
        // =========================================================================

        [HttpPost]
        public async Task<IActionResult> SendOtp(string target)
        {
            if (string.IsNullOrEmpty(target) || !target.Contains("@"))
            {
                ModelState.AddModelError("", "Hệ thống không nhận diện được định dạng Email. Vui lòng kiểm tra lại.");
                return View("~/Views/Auth/Login.cshtml");
            }

            string randomOtp = new Random().Next(100000, 999999).ToString();

            var otpEntry = new OtpVerification
            {
                Receiver = target, // FIX TỪ 'Target' THÀNH 'Receiver' ĐỂ ĐỒNG BỘ MODEL VÀ DB
                OtpCode = randomOtp,
                ExpiredAt = DateTime.Now.AddMinutes(5),
                IsUsed = false
            };

            _context.OtpVerifications.Add(otpEntry);
            await _context.SaveChangesAsync();

            try
            {
                await SendOtpEmailReal(target, randomOtp);

                TempData["UserTarget"] = target;
                TempData["IsRegisterFlow"] = false;

                return RedirectToAction("VerifyOtp");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hệ thống gửi thư gặp sự cố kỹ thuật: {ex.Message}. Vui lòng thử lại sau.");
                return View("~/Views/Auth/Login.cshtml");
            }
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            ViewBag.Target = TempData["UserTarget"];
            return View("~/Views/Auth/VerifyOtp.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOtp(string target, string otpCode)
        {
            if (string.IsNullOrEmpty(otpCode))
            {
                ModelState.AddModelError("", "Quý khách vui lòng nhập mã số OTP gồm 6 chữ số.");
                ViewBag.Target = target;
                return View("~/Views/Auth/VerifyOtp.cshtml");
            }

            var validOtp = await _context.OtpVerifications
                .Where(o => o.Receiver == target && o.OtpCode == otpCode && !o.IsUsed && o.ExpiredAt > DateTime.Now) // FIX TỪ 'o.Target' THÀNH 'o.Receiver'
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();

            if (validOtp == null)
            {
                ModelState.AddModelError("", "Mã xác thực OTP không chính xác hoặc đã hết hiệu lực sử dụng.");
                ViewBag.Target = target;
                return View("~/Views/Auth/VerifyOtp.cshtml");
            }

            validOtp.IsUsed = true;
            await _context.SaveChangesAsync();

            // LƯU TÀI KHOẢN MỚI SAU KHI ĐĂNG KÝ VÀ XÁC THỰC OTP THÀNH CÔNG
            if (TempData["IsRegisterFlow"] != null && (bool)TempData["IsRegisterFlow"])
            {
                try
                {
                    var newUser = new User
                    {
                        Username = TempData["Reg_Username"]?.ToString() ?? "",
                        PasswordHash = TempData["Reg_Password"]?.ToString() ?? "",
                        Email = TempData["Reg_Email"]?.ToString() ?? "",
                        FullName = TempData["Reg_FullName"]?.ToString() ?? "",
                        Phone = TempData["Reg_Phone"]?.ToString() ?? "",
                        Role = "Patient",
                        CreatedAt = DateTime.Now
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Chúc mừng Quý khách đã tạo tài khoản thành viên thành công tại Hệ thống Phòng Khám VIP!";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    var innerMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError("", $"Lỗi hệ thống khi lưu hồ sơ: {innerMsg}");
                    ViewBag.Target = target;
                    return View("~/Views/Auth/VerifyOtp.cshtml");
                }
            }

            // LOGIC ĐĂNG NHẬP QUA OTP
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == target);

            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản liên kết với địa chỉ Email này không tồn tại trên hệ thống.");
                ViewBag.Target = target;
                return View("~/Views/Auth/VerifyOtp.cshtml");
            }

            await SignInUserAsync(user);
            return RedirectUserByRole(user.Role);
        }

        // =========================================================================
        // 4. CHỨC NĂNG ĐĂNG XUẤT HỆ THỐNG (LOGOUT FLOW)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // =========================================================================
        // PHƯƠNG THỨC BỔ TRỢ (HELPER METHODS)
        // =========================================================================

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "Patient")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        private IActionResult RedirectUserByRole(string? role)
        {
            if (role == "Admin")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }
            else if (role == "Doctor")
            {
                return RedirectToAction("Index", "DoctorSchedule");
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task SendOtpEmailReal(string toEmail, string otp)
        {
            string fromEmail = "pthituyet298@gmail.com";
            string appPassword = "yljnhhpryqodjodd";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PHÒNG KHÁM VIP", fromEmail));
            message.To.Add(new MailboxAddress("Khách hàng", toEmail));
            message.Subject = $"[{otp}] - Mã xác thực thông tin tài khoản tại Hệ thống Phòng Khám VIP";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px;'>
                        <h2 style='color: #008094; text-align: center; margin-bottom: 25px;'>HỆ THỐNG PHÒNG KHÁM VIP</h2>
                        <p>Kính chào Quý khách,</p>
                        <p>Hệ thống nhận được yêu cầu cung cấp mã OTP để xác thực thông tin tài khoản thành viên từ phía Quý khách.</p>
                        <div style='background-color: #f4fbfd; border: 1px dashed #008094; padding: 18px; text-align: center; margin: 25px 0; border-radius: 6px;'>
                            <p style='margin: 0; font-size: 14px; color: #555555;'>Mã xác thực của Quý khách là:</p>
                            <span style='font-size: 28px; font-weight: bold; color: #008b9b; letter-spacing: 5px; display: block; margin-top: 10px;'>{otp}</span>
                        </div>
                        <p style='color: #d9534f; font-size: 13px; font-style: italic;'>* Lưu ý: Mã OTP này có thời hạn sử dụng trong vòng 5 phút và chỉ áp dụng cho 1 lần xác thực duy nhất. Để đảm bảo an toàn, Quý khách tuyệt đối không cung cấp mã này cho bất kỳ ai.</p>
                        <hr style='border: 0; border-top: 1px solid #eeeeee; margin: 25px 0;'/>
                        <p style='font-size: 11px; color: #999999; text-align: center; margin: 0;'>Đây là thông điệp tự động từ hệ thống quản lý phòng khám, vui lòng không phản hồi trực tiếp email này.</p>
                    </div>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, appPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}