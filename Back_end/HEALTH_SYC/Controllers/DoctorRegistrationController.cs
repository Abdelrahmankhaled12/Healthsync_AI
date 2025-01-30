using HEALTH_SYC.Models;
using Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorRegistrationController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public DoctorRegistrationController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
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

                // Generate confirmation token
                string confirmationToken = GenerateConfirmationToken(request.Username, "Doctor");

                // Create a new doctor record
                var newDoctor = new Doctor
                {
                    Name = request.Username,
                    Email = request.Email,
                    Password = passwordHash,
                    Location = request.Location,
                    RegionId = request.RegionId,
                    DepartmentId = request.DepartmentId,
                    ConfirmationToken = confirmationToken,
                    IsConfirmed = false,
                    Role = "Doctor" // Set the role to "Doctor"
                };

                // Save the new doctor to the database
                _db.Doctors.Add(newDoctor);
                await _db.SaveChangesAsync();

                // Send confirmation email
                SendConfirmationEmail(request.Email, request.Username, confirmationToken);

                return Ok("Doctor registered successfully. Please check your email for confirmation.");
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
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*[\W]).{8,}$");
            return regex.IsMatch(password);
        }

        // Generate confirmation token (JWT)
        private string GenerateConfirmationToken(string username, string role)
        {
            var secretKey = _configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT secret key is not configured in appsettings.json.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("username", username),
                new Claim("role", role),  // Add role as claim here
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = credentials,
                Issuer = "HEALTH_SYC",
                Audience = "HEALTH_SYC"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Send confirmation email with the token link
        private void SendConfirmationEmail(string email, string username, string confirmationToken)
        {
            try
            {
                string fromEmail = "healthsyc@gmail.com"; // Replace with your email
                string emailPassword = "huhv iqtx ttit cowh";    // Replace with your email password
                string smtpHost = "smtp.gmail.com";        // SMTP host
                int smtpPort = 587;                       // SMTP port

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "HEALTH_SYC Team"),
                    Subject = "Email Confirmation",
                    IsBodyHtml = true,
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; text-align: center;' >
                            <img src='https://i.imgur.com/JiImrrK.jpeg' alt='HEALTH_SYC Logo' style='max-width: 150px; margin-bottom: 20px;' />
                            <h2>Welcome to HEALTH_SYC, {username}!</h2>
                            <p>Thank you for registering. Please confirm your email to activate your account.</p>
                            <a href='http://localhost:5000/api/DoctorRegistration/confirm?token={confirmationToken}' 
                               style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;' >
                                Click here to confirm your email
                            </a>
                            <br /><br />
                            <p>Best regards,<br />The HEALTH_SYC Team</p>
                        </div>"
                };
                mailMessage.To.Add(email);

                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(fromEmail, emailPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        // Confirm the email address by token
        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmDoctorEmail(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Invalid token.");
                }

                var username = DecodeToken(token);

                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Invalid token.");
                }

                // Retrieve the doctor from the database
                var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Name == username);

                if (doctor == null)
                {
                    return NotFound("Doctor not found.");
                }

                // Confirm email
                doctor.IsConfirmed = true;
                await _db.SaveChangesAsync();

                return Ok("Doctor email confirmed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Decode token and extract username
        private string DecodeToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "HEALTH_SYC",
                    ValidAudience = "HEALTH_SYC"
                };

                var principal = tokenHandler.ValidateToken(token, validations, out _);
                var username = principal.FindFirst("username")?.Value;  // Extract username from token

                return username;
            }
            catch
            {
                return null;
            }
        }
    }
}
