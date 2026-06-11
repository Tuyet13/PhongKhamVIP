using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.System
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string ActionBy { get; set; } // Username thực hiện hành động

        [Required, StringLength(100)]
        public string ActionType { get; set; } // "Sửa bệnh án", "Xóa thuốc", "Xuất hóa đơn"

        [Required]
        public string Description { get; set; } // Mô tả chi tiết thay đổi dữ liệu

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
