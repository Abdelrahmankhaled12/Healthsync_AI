using HEALTH_SYC.Models;

namespace HEALTH_SYC.DTOs
{
    public class AppointmentDto
    {
        public required int AppointmentId { get; set; }
      
        public required int PatientId { get; set; }
        public required string PatientName { get; set; }
        public required int DoctorId { get; set; }
        public required string DoctorName { get; set; }
        public  string? Region { get; set; }
    }
}
