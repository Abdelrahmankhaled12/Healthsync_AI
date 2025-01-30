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

        // API to view all bookings for the logged-in doctor
        [HttpGet("view-bookings")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                // Get the logged-in doctor's email and role from the JWT token
                var doctorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(doctorEmail) || userRole != "Doctor")
                {
                    return Unauthorized("Doctor is not logged in or does not have the correct role.");
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
                if (!bookings.Any())
                {
                    return NotFound("No bookings found for this doctor.");
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // API to manage bookings (Accept, Reject, Reschedule, Postpone)
        [HttpPost("update-booking-status")]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] BookingActionDto request)
        {
            try
            {
                // Get the logged-in doctor's email and role
                var doctorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(doctorEmail) || userRole != "Doctor")
                {
                    return Unauthorized("Doctor is not logged in or does not have the correct role.");
                }

                // Verify if the doctor exists in the database
                var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Email == doctorEmail);
                if (doctor == null)
                {
                    return Unauthorized("Doctor does not exist.");
                }

                // Find the booking
                var booking = await _db.Appointments
                    .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.DoctorId == doctor.Id);

                if (booking == null)
                {
                    return NotFound("Booking not found or does not belong to the logged-in doctor.");
                }

                // Ensure the booking is in a valid state to be updated
                if (booking.Status != "Pending" && booking.Status != "Confirmed")
                {
                    return BadRequest("Booking cannot be updated as it is not in a valid state.");
                }

                // Handle booking actions
                switch (request.Action.ToLower())
                {
                    case "accept":
                        booking.Status = "Confirmed";
                        booking.Notes = "Appointment confirmed by doctor.";
                        break;

                    case "reject":
                        booking.Status = "Rejected";
                        booking.Notes = request.Reason ?? "Rejected by doctor.";
                        break;

                    case "reschedule":
                        if (request.NewDate == null)
                        {
                            return BadRequest("New date is required for rescheduling.");
                        }
                        booking.Status = "Rescheduled";
                        booking.Date = request.NewDate.Value;
                        booking.Notes = "Rescheduled by doctor.";
                        break;

                    case "postpone":
                        booking.Status = "Postponed";
                        booking.Notes = request.Reason ?? "Postponed by doctor.";
                        break;

                    default:
                        return BadRequest("Invalid action. Allowed actions: Accept, Reject, Reschedule, Postpone.");
                }

                booking.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();

                // Send notification to patient (Simulated with logging)
                Console.WriteLine($"Notification: Booking {booking.Id} updated to {booking.Status}");

                return Ok(new { Message = "Booking status updated successfully", NewStatus = booking.Status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
