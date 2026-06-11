using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Clinical
{
    public class Medicine
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Unit { get; set; } // Viên, Chai, Gói

        [Required]
        public decimal Price { get; set; } // Giá bán lẻ

        [Required]
        public int StockQuantity { get; set; } // Số lượng tồn kho

        [StringLength(200)]
        public string Description { get; set; }
    }
}
