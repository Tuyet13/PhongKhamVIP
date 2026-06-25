using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PhongKhamVIP.Models.Users;

namespace PhongKhamVIP.Models.Finance
{
    public class Salary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        // Đổi các trường số tiền thành decimal để tính toán và định dạng tiền tệ mượt mà
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }

        // Khởi tạo chuỗi rỗng để triệt tiêu hoàn toàn cảnh báo CS8618 (Non-nullable property must contain a non-null value)
        public string Status { get; set; } = "Unpaid";

        public DateTime? PaidDate { get; set; }
    }
}