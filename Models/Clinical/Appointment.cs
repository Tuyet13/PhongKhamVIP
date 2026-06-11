using PhongKhamVIP.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Clinical
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan TimeSlot { get; set; } // Giờ khám cụ thể

        [StringLength(10)]
        public string QueueNumber { get; set; } // Số thứ tự khám

        [Required, StringLength(20)]
        public string Status { get; set; } // Pending, Confirmed, Completed, Cancelled

        [StringLength(200)]
        public string Notes { get; set; }
    }
}
