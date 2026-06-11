using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Clinical
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedicalRecordId { get; set; }

        [ForeignKey("MedicalRecordId")]
        public virtual MedicalRecord MedicalRecord { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string DoctorNotes { get; set; } // Ghi chú của bác sĩ dặn bệnh nhân

        // Mối quan hệ 1-N sang chi tiết đơn thuốc
        public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; }
    }
}
