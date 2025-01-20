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
    public class PatientRegistrationController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PatientRegistrationController(AppDbContext db)
        {
            _db = db;
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
                var existingEmail = await _db.Patients.AnyAsync(p => p.Email == request.Email);
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
                    Adress = request.Address,
                    Age = request.Age
                };

                // Save to database
                _db.Patients.Add(newPatient);
                await _db.SaveChangesAsync();

                return Ok("Patient registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Email validation
        private bool IsValidEmail(string email)
        {
            return email.Contains("@");
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
            return Regex.IsMatch(phone, @"^\+?\d{7,}$");
        }

        // Age validation
        private bool IsValidAge(int age)
        {
            return age > 0 && age < 120;
        }
    }
}
