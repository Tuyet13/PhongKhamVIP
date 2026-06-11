using PhongKhamVIP.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Clinical
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [Required]
        public DateTime DateOfVisit { get; set; }

        [Required, StringLength(500)]
        public string Symptoms { get; set; } // Triệu chứng lâm sàng

        [Required, StringLength(200)]
        public string Diagnosis { get; set; } // Chẩn đoán bệnh (ICD-10)

        public string TreatmentPlan { get; set; } // Hướng điều trị

        // Mối quan hệ liên kết sang Đơn thuốc
        public virtual Prescription Prescription { get; set; }
    }
}
