using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PhongKhamVIP.Models.Users;

namespace PhongKhamVIP.Models.Finance
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Khớp với model bạn gửi

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public TimeSpan? CheckIn { get; set; } // Sử dụng TimeSpan như model của bạn

        public TimeSpan? CheckOut { get; set; }

        [Required]
        public string Status { get; set; } = "Absent";

        public string? Note { get; set; }
    }
}