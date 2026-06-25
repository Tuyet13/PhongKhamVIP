using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using PhongKhamVIP.Models.Clinical;
namespace PhongKhamVIP.Models.Users
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        // Khớp hoàn toàn với trường dữ liệu gốc trong Database của bạn
        public string Phone { get; set; }

        // Khắc phục lỗi thiếu thuộc tính 'PhoneNumber' ở dòng 26 file EmployeeController
        [NotMapped]
        public string PhoneNumber
        {
            get => Phone;
            set => Phone = value;
        }

        public string Email { get; set; }

        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }
        [NotMapped]
        public bool IsActive { get; set; } = true;
    }
}