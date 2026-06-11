using PhongKhamVIP.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.System
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, StringLength(250)]
        public string Reason { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } // Pending, Approved, Rejected
    }
}
