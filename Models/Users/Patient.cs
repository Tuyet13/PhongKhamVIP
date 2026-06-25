using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PhongKhamVIP.Models.Clinical;
using PhongKhamVIP.Models.Finance;

namespace PhongKhamVIP.Models.Users
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } // Trường bắt buộc

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? HealthInsuranceNumber { get; set; }

        [StringLength(500)]
        public string? MedicalHistory { get; set; }

        public virtual ICollection<Appointment>? Appointments { get; set; }
        public virtual ICollection<Invoice>? Invoices { get; set; }
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; }
    }
}