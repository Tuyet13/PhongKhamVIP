using Microsoft.EntityFrameworkCore;
using PhongKhamVIP.Models;
using PhongKhamVIP.Models.Users;
using PhongKhamVIP.Models.Clinical;
using PhongKhamVIP.Models.Finance;
using PhongKhamVIP.Models.System;
namespace PhongKhamVIP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 1. USERS
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }

        // 2. CLINICAL
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }

        // 3. FINANCE
        public DbSet<Service> Services { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Salary> Salaries { get; set; }
      

        // 4. SYSTEM
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        // Trong class ApplicationDbContext
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================================
            // CẤU HÌNH QUAN HỆ BẢNG (RELATIONSHIPS)
            // =========================================================================

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.MedicalRecord)
                .WithMany()
                .HasForeignKey(i => i.MedicalRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany() 
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================================
            // CẤU HÌNH ĐỒNG BỘ TRÁNH LỖI HỆ THỐNG (FIXING MISSING DB COLUMNS)
            // =========================================================================

            // Ép EF Core bỏ qua các thuộc tính ảo của bảng Users khi biên dịch câu lệnh SQL
            modelBuilder.Entity<User>(entity =>
            {
                entity.Ignore(u => u.IsActive);
                entity.Ignore(u => u.PhoneNumber);
            });
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.User)
                .WithMany() // Nếu User không có collection LeaveRequests thì để trống
    .           HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.Cascade); // Khi xóa User thì xóa đơn nghỉ phép của họ
            // Loại bỏ hoàn toàn cấu hình ép buộc cũ của OtpVerification để tránh xung đột Metadata cache
            // EF Core sẽ tự động ánh xạ thuộc tính Receiver -> cột Receiver dưới SQL một cách tự nhiên.
        }
    }
}