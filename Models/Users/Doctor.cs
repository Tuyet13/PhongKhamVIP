using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// --- THÊM CÁC DÒNG USING NÀY ĐỂ KẾT NỐI ĐẾN THƯ MỤC CLINICAL VÀ SYSTEM ---
using PhongKhamVIP.Models.Clinical;
using PhongKhamVIP.Models.Finance;
using PhongKhamVIP.Models.System;

namespace PhongKhamVIP.Models.Users
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int SpecialtyId { get; set; }

        [ForeignKey("SpecialtyId")]
        public virtual Specialty Specialty { get; set; }

        [StringLength(500)]
        public string Biography { get; set; }

        // Mối quan hệ liên kết
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
    }
}