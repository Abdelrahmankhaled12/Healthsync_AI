using HEALTH_SYC.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Booking
{
    [Key] // Primary Key
    public int Id { get; set; }

    [ForeignKey("Doctor")] 
    public int DoctorId { get; set; }

    [ForeignKey("Patient")] 
    public int PatientId { get; set; }

    public virtual Patient Patient { get; set; } // Navigation property to access patient details

    [Required] 
    public DateTime AppointmentDate { get; set; }

    [Required] 
    public TimeSpan AppointmentTime { get; set; } // Add AppointmentTime property

    [Required] 
    [MaxLength(50)] 
    public string Status { get; set; }

    public string? Notes { get; set; } 

    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; } // Can be updated later
}
