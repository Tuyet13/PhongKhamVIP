using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// --- THÊM CÁC DÒNG USING NÀY ĐỂ LIÊN KẾT THƯ MỤC ---
using PhongKhamVIP.Models.Clinical;
using PhongKhamVIP.Models.Finance;

namespace PhongKhamVIP.Models.Users
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; } // Nullable nếu bệnh nhân vãng lai chưa tạo tài khoản web

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(100)]
        public string HealthInsuranceNumber { get; set; } // Mã BHYT

        [StringLength(500)]
        public string MedicalHistory { get; set; } // Tiền sử bệnh/dị ứng thuốc

        // Mối quan hệ liên kết
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}