using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Finance
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } // Công khám, Siêu âm, Xét nghiệm máu...

        [Required]
        public decimal BasePrice { get; set; } // Giá tiền dịch vụ

        [StringLength(200)]
        public string Description { get; set; }
    }
}
