using HEALTH_SYC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HEALTH_SYC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure the doctor is logged in
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BookingController(AppDbContext db)
        {
            _db = db;
        }

        // API to view all bookings
        [HttpGet("view-bookings")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                // Get the logged-in doctor's email from the JWT token
                var doctorEmail = User.FindFirst(ClaimTypes.Email)?.Value;  // Use ClaimTypes.Email here
                if (string.IsNullOrEmpty(doctorEmail))
                {
                    return Unauthorized("Doctor is not logged in.");
                }

                // Verify if the doctor exists in the database
                var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Email == doctorEmail);
                if (doctor == null)
                {
                    return Unauthorized("Doctor does not exist.");
                }
                // Fetch all bookings for the logged-in doctor
                var bookings = await _db.Appointments
                    .Where(b => b.DoctorId == doctor.Id)
                    .Select(b => new
                    {
                        BookingId = b.Id,
                        PatientName = b.Patient.Name, 
                        PatientContact = b.Patient.Phone,
                        AppointmentDate = b.Date,
                        Status = b.Status,
                        Notes = b.Notes
                    })
                    .ToListAsync();

                // If no bookings found
                if (bookings.Count == 0)
                {
                    return NotFound("No bookings found for this doctor.");
                }

                // Return the list of bookings
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
