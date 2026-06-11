using PhongKhamVIP.Models.Users;

namespace PhongKhamVIP.Models.Finance
{
    public class Attendance
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
