using System;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.System
{
    public class OtpVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Receiver { get; set; } // Trùng khớp với cột Receiver dưới DB

        [Required]
        [StringLength(6)]
        public string OtpCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiredAt { get; set; }

        public bool IsUsed { get; set; } = false;
    }
}