using PhongKhamVIP.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhongKhamVIP.Models.Finance
{
    public class Salary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public decimal BaseSalary { get; set; } // Lương cơ bản

        public decimal Bonus { get; set; } // Thưởng KPI ca khám

        public decimal Deductions { get; set; } // Trừ tiền (Nghỉ không phép, đi muộn)

        [Required]
        public decimal NetSalary { get; set; } // Lương thực nhận

        public DateTime PaidDate { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } // Pending, Paid
    }
}
