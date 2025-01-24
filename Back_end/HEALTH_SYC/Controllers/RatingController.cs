using HEALTH_SYC.DTOs;
using HEALTH_SYC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("AddComplain")]
        public async Task<IActionResult> AddComplain(ComplainDto complainDto)
        {
            Complain complain = new()
            {
                Name = complainDto.Name,
                Phone = complainDto.Phone,
                Email = complainDto.Email,
                Message = complainDto.Message,
                Status = -1 ,
            };
            await _db.Complains.AddAsync(complain);
            _db.SaveChanges();
            return Ok(new { Message = "new complain created successfully", Id = complain.Id });
        }

        [HttpGet("GetAllComplains")]
        public async Task<IActionResult> GetAllComplains()
        {
            var complains = await _db.Complains.ToListAsync();

            var list = new List<ComplainDto>();

            foreach (var complain in complains)
            {

                ComplainDto complainDto = new ComplainDto()
                {
                    Name = complain.Name ,
                    Phone = complain.Phone ,
                    Email = complain.Email ,
                    Message = complain.Message ,
                    Status = complain.Status ,
                };
                list.Add(complainDto);

            }
            return Ok(list);
        }

        [HttpPost("ResponseComplain")]
        public async Task<IActionResult> ResponseComplain(ResponseComplainDto responseComplainDto)
        {
            var complain = await _db.Complains.SingleOrDefaultAsync(x => x.Id == responseComplainDto.Id);
            if (complain == null)
            {
                return NotFound($"complain Id {responseComplainDto.Id} not exist");
            }
            if (responseComplainDto.ZeroOrOne!=0 && responseComplainDto.ZeroOrOne!=1)
            {
                return NotFound($"Status should be zero or one");
            }

            complain.Status = responseComplainDto.ZeroOrOne;
            _db.SaveChanges();
            return Ok(complain);
        }
    }
}
