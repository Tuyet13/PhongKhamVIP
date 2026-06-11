using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Clinical
{
    public class PrescriptionDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [ForeignKey("MedicineId")]
        public virtual Medicine Medicine { get; set; }

        [Required]
        public int Quantity { get; set; } // Số lượng cấp phát

        [Required, StringLength(200)]
        public string Dosage { get; set; } // Liều dùng (Ví dụ: Sáng 1 viên, tối 1 viên sau ăn)
    }
}
