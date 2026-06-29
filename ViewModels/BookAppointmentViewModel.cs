using Microsoft.AspNetCore.Mvc.Rendering;

namespace PhongKhamVIP.ViewModels
{
    public class BookAppointmentViewModel
    {
        public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Note { get; set; } // Dùng string? để cho phép để trống
    public List<SelectListItem>? Doctors { get; set; }
    }
}
