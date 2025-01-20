using HEALTH_SYC.Models;
using Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorRegistrationController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DoctorRegistrationController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDoctor(DoctorRegisterDto request)
        {
            try
            {
                // Validate email format
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest("Invalid email format. Email must contain '@'.");
                }

                // Validate password strength
                if (!IsValidPassword(request.Password))
                {
                    return BadRequest("Password must be at least 8 characters long, contain at least one uppercase letter, and one special character.");
                }

                // Check if the email already exists
                var existingEmail = await _db.Doctors.AnyAsync(d => d.Email == request.Email);
                if (existingEmail)
                {
                    return BadRequest("Email already exists.");
                }

                // Hash the password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create a new doctor record
                var newDoctor = new Doctor
                {
                    Name = request.Username,
                    Email = request.Email,
                    Password = passwordHash,
                    Location = request.Location,
                    RegionId = request.RegionId,
                    DepartmentId = request.DepartmentId
                };

                // Save the new doctor to the database
                _db.Doctors.Add(newDoctor);
                await _db.SaveChangesAsync();

                return Ok("Doctor registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Helper method to validate email
        private bool IsValidEmail(string email)
        {
            return email.Contains("@");
        }

        // Helper method to validate password
        private bool IsValidPassword(string password)
        {
            // Regular expression to check for at least one uppercase letter, one special character, and minimum 8 characters
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*[\W]).{8,}$");
            return regex.IsMatch(password);
        }
    }
}
