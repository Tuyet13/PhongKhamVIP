using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Users
{
    public class Receptionist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [StringLength(50)]
        public string ShiftInfo { get; set; } // Ca trực
    }
}
