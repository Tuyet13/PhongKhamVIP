using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Finance
{
    public class InvoiceDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [StringLength(150)]
        public string ItemName { get; set; } // Lưu text trực tiếp: "Tiền thuốc Paracetamol" hoặc "Phí siêu âm"

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal SubTotal { get; set; } // Thành tiền = Quantity * UnitPrice
    }
}
