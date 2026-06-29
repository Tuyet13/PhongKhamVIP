using PhongKhamVIP.Models.Clinical;
using PhongKhamVIP.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Finance
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public int? MedicalRecordId { get; set; } // Liên kết để biết hóa đơn này thu tiền cho ca khám nào

        [ForeignKey("MedicalRecordId")]
        public virtual MedicalRecord MedicalRecord { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } // Tổng tiền trước giảm trừ

        public decimal DiscountAmount { get; set; } // Tiền được giảm (BHYT hoặc Khuyến mãi)

        [Required]
        public decimal FinalAmount { get; set; } // Số tiền thực tế phải trả

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required, StringLength(20)]
        public string PaymentMethod { get; set; } // Cash, VietQR, VNPay

        [Required, StringLength(20)]
        public string Status { get; set; } // Unpaid, Paid, Refunded

        // Mối quan hệ 1-N sang chi tiết các khoản thu
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
