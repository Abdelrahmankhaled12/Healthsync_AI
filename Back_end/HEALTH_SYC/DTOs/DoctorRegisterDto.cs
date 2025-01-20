public class DoctorRegisterDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Location { get; set; }
    public required int RegionId { get; set; }
    public required int DepartmentId { get; set; }
}
