using HEALTH_SYC.DTOs;
using HEALTH_SYC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HEALTH_SYC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RatingController(AppDbContext db)
        {
            _db = db;
        }

        // API to submit a new complaint (Available to any user)
        [HttpPost("AddComplain")]
        public async Task<IActionResult> AddComplain(ComplainDto complainDto)
        {
            Complain complain = new()
            {
                Name = complainDto.Name,
                Phone = complainDto.Phone,
                Email = complainDto.Email,
                Message = complainDto.Message,
                Status = -1,
            };
            await _db.Complains.AddAsync(complain);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "New complaint created successfully", Id = complain.Id });
        }

        // API to get all complaints (Available only to Admin)
        [HttpGet("GetAllComplains")]
        [Authorize(Roles = "Admin")] // Only Admin can view all complaints
        public async Task<IActionResult> GetAllComplains()
        {
            var complains = await _db.Complains.ToListAsync();
            var list = new List<ComplainDto>();

            foreach (var complain in complains)
            {
                ComplainDto complainDto = new ComplainDto()
                {
                    Name = complain.Name,
                    Phone = complain.Phone,
                    Email = complain.Email,
                    Message = complain.Message,
                    Status = complain.Status,
                };
                list.Add(complainDto);
            }
            return Ok(list);
        }

        // API to respond to a complaint (Available only to Admin or Doctor)
        [HttpPost("ResponseComplain")]
        [Authorize(Roles = "Admin,Doctor")] // Admin and Doctor can respond to complaints
        public async Task<IActionResult> ResponseComplain(ResponseComplainDto responseComplainDto)
        {
            var complain = await _db.Complains.SingleOrDefaultAsync(x => x.Id == responseComplainDto.Id);
            if (complain == null)
            {
                return NotFound($"Complaint Id {responseComplainDto.Id} not found");
            }

            if (responseComplainDto.ZeroOrOne != 0 && responseComplainDto.ZeroOrOne != 1)
            {
                return BadRequest("Status should be zero or one");
            }

            complain.Status = responseComplainDto.ZeroOrOne;
            await _db.SaveChangesAsync();
            return Ok(complain);
        }
    }
}
