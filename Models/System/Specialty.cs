using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.System
{
    public class Specialty
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } // Khoa Nội, Khoa Nhi, Răng Hàm Mặt...

        [StringLength(200)]
        public string Description { get; set; }
    }
}
