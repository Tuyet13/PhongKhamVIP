using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PhongKhamVIP.Models.Users; // Đảm bảo đã import namespace chứa lớp Patient

namespace PhongKhamVIP.Models.Clinical
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } // ĐỔI TỪ User SANG Patient

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Prescription { get; set; }
        public string Symptoms { get; set; }

        [Required, StringLength(200)]
        public string Diagnosis { get; set; }

        public string TreatmentPlan { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}