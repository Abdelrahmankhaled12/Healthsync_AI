using HEALTH_SYC.Models; 
using Login.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public LoginController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            try
            {
                // Search for the user in Admins, Doctors, and Patients tables
                var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Email == request.Email);
                var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Email == request.Email);
                var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Email == request.Email);

                // Check if the user exists in any of the tables
                if (admin == null && doctor == null && patient == null)
                {
                    return BadRequest("Email not found.");
                }

                // Verify password and return a JWT token if valid
                if (admin != null && BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
                {
                    string token = CreateToken(admin.Name, "Admin");
                    return Ok(new LoginResult { Token = token, RoleType = "Admin", Id = admin.Id });
                }

                if (doctor != null && BCrypt.Net.BCrypt.Verify(request.Password, doctor.Password))
                {
                    string token = CreateToken(doctor.Name, "Doctor");
                    return Ok(new LoginResult { Token = token, RoleType = "Doctor", Id = doctor.Id });
                }

                if (patient != null && BCrypt.Net.BCrypt.Verify(request.Password, patient.Password))
                {
                    string token = CreateToken(patient.Name, "Patient");
                    return Ok(new LoginResult { Token = token, RoleType = "Patient", Id = patient.Id });
                }

                return BadRequest("Incorrect password.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private string CreateToken(string username, string role)
        {
            // Define claims for the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            // Get the secret key from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Generate the token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
