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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- QUAN HỆ CÁC BẢNG ---
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
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CẤU HÌNH DECIMAL (Khắc phục lỗi Precision) ---
            modelBuilder.Entity<Invoice>().Property(i => i.DiscountAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.FinalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceDetail>().Property(i => i.SubTotal).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceDetail>().Property(i => i.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Salary>().Property(s => s.BaseSalary).HasPrecision(18, 2);
            modelBuilder.Entity<Salary>().Property(s => s.Bonus).HasPrecision(18, 2);
            modelBuilder.Entity<Salary>().Property(s => s.Deductions).HasPrecision(18, 2);
            modelBuilder.Entity<Salary>().Property(s => s.NetSalary).HasPrecision(18, 2);
            modelBuilder.Entity<Service>().Property(s => s.BasePrice).HasPrecision(18, 2);

            // --- BỎ QUA CỘT ẢO ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.Ignore(u => u.IsActive);
                entity.Ignore(u => u.PhoneNumber);
            });

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}