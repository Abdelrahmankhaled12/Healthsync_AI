using System.ComponentModel.DataAnnotations;

namespace HEALTH_SYC.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = "Admin";  
        public bool IsConfirmed { get; set; } = false;  
        public string ConfirmationToken { get; set; }
    }
}
