using HEALTH_SYC.Models;
using Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientRegistrationController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public PatientRegistrationController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterPatient(PatientRegisterDto request)
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
                    return BadRequest("Password must be at least 8 characters long, contain at least one letter, one special character, and one uppercase letter.");
                }

                // Validate phone number
                if (!IsValidPhone(request.Phone))
                {
                    return BadRequest("Phone number must contain only digits, may start with '+', and must be at least 7 digits long.");
                }

                // Validate age
                if (!IsValidAge(request.Age))
                {
                    return BadRequest("Age must be greater than 0 and less than 120.");
                }

                // Check if email already exists
                var existingEmail = await _db.Patients
                    .AsNoTracking()
                    .AnyAsync(p => p.Email == request.Email);
                if (existingEmail)
                {
                    return BadRequest("Email already exists.");
                }

                // Hash password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new patient
                var newPatient = new Patient
                {
                    Name = request.Username,
                    Email = request.Email,
                    Password = passwordHash,
                    Phone = request.Phone,
                    Address = request.Address,
                    Age = request.Age,
                    Role = "Patient"  // Set role for the patient
                };

                // Save to database
                _db.Patients.Add(newPatient);
                await _db.SaveChangesAsync();

                // Create JWT token for the registered patient
                var token = CreateToken(newPatient.Email, "Patient");

                // Return response with Patient ID and token
                return Ok(new { Message = "Patient registered successfully.", PatientId = newPatient.Id, Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Email validation
        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Password validation
        private bool IsValidPassword(string password)
        {
            var hasLetter = Regex.IsMatch(password, "[a-zA-Z]");
            var hasSpecialChar = Regex.IsMatch(password, "[^a-zA-Z0-9]");
            var hasUpperCase = Regex.IsMatch(password, "[A-Z]");
            return password.Length >= 8 && hasLetter && hasSpecialChar && hasUpperCase;
        }

        // Phone number validation
        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\+?[0-9]{7,15}$");
        }

        // Age validation
        private bool IsValidAge(int age)
        {
            return age > 0 && age < 120;
        }

        // Method to create JWT token
        private string CreateToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
