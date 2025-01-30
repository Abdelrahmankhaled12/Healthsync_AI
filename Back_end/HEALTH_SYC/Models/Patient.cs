namespace HEALTH_SYC.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public bool IsConfirmed { get; set; }
        public string Role { get; set; } = "Patient"; 
    }
}
