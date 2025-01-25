using System.ComponentModel.DataAnnotations.Schema;

namespace HEALTH_SYC.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? Location  { get; set; }
        public string? Phone { get; set; }
        [ForeignKey("Region")]
        public int? RegionId { get; set; }
       
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public string ConfirmationToken { get; set; }  // Token for email confirmation
        public bool IsConfirmed { get; set; } = false; // Indicates if the email is confirmed

    }
}
