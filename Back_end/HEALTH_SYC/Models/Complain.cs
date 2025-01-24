namespace HEALTH_SYC.Models
{
    public class Complain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
