using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKhamVIP.Models.Clinical;
using PhongKhamVIP.Models.System;

namespace PhongKhamVIP.ViewModels
{
    public class PatientDashboardViewModel
    {
        public PhongKhamVIP.Models.Clinical.Appointment? UpcomingAppointment { get; set; }
       // public List<PhongKhamVIP.Models.Clinical.Appointment> RecentAppointments { get; set; } = new();
       // public List<PhongKhamVIP.Models.System.Notification> Notifications { get; set; } = new();
        public List<Appointment> RecentAppointments { get; set; } = new List<Appointment>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public List<SelectListItem> Doctors { get; set; } = new();
    }
}