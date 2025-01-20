namespace Login.Models
{
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResult
    {
        public string Token { get; set; }
        public string RoleType { get; set; }
        public int Id { get; set; }
    }
}
