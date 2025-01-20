using System.ComponentModel.DataAnnotations.Schema;

namespace HEALTH_SYC.Models
{
    public class Rating
    {
        public int Id { get; set; }
        [ForeignKey("Patient")]
        public int PatienId { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public string? Comment { get; set; }
    }
}
