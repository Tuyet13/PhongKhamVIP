using PhongKhamVIP.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.System
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } // Phải có dòng này!

        public User User { get; set; } // Liên kết cho EF Core
    }
}