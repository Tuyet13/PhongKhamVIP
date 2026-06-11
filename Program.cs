using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Data;
// Thêm thư viện chứa cấu hình Cookie mặc định
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PhongKhamVIP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================================
            // 1. KÍCH HOẠT KẾT NỐI DATABASE (CODE-FIRST)
            // ==========================================================
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==========================================================
            // [BỔ SUNG] 2. ĐĂNG KÝ DỊCH VỤ XÁC THỰC COOKIE AUTHENTICATION
            // ==========================================================
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";   // Tuyến đường đá về nếu chưa đăng nhập mà cố vào trang cấm
                    options.LogoutPath = "/Account/Logout"; // Tuyến đường xử lý xóa sạch Cookie phiên làm việc
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // ĐÂY RỒI: Dùng hàm này để đọc các file trong thư mục wwwroot trên .NET 8
            app.UseStaticFiles();

            app.UseRouting();

            // ==========================================================
            // [BỔ SUNG] 3. KÍCH HOẠT MIDDLEWARE XÁC THỰC TÀI KHOẢN
            // LƯU Ý: Bắt buộc UseAuthentication phải đứng TRƯỚC UseAuthorization
            // ==========================================================
            app.UseAuthentication();
            app.UseAuthorization();

            // ==========================================================
            // 4. CẤU HÌNH ROUTE CHO CÁC AREAS (ADMIN, DOCTOR, RECEPTIONIST)
            // ==========================================================
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            // Tuyến đường mặc định cho các trang công cộng bên ngoài (Home, Login...)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}