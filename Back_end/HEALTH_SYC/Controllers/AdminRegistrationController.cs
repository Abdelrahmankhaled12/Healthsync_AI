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
    public class AdminRegistrationController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminRegistrationController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterDto request)
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

                var existingEmail = await _db.Admins.AnyAsync(a => a.Email == request.Email);
                if (existingEmail)
                {
                    return BadRequest("Email already exists.");
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newAdmin = new Admin
                {
                    Name = request.Username,
                    Email = request.Email,
                    Password = passwordHash
                };

                _db.Admins.Add(newAdmin);
                await _db.SaveChangesAsync();

                return Ok("Admin registered successfully.");
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
