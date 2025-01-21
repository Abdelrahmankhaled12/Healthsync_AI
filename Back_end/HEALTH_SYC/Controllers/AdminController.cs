using HEALTH_SYC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using HEALTH_SYC.DTOs;
using System.Numerics;
namespace HEALTH_SYC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [HttpDelete("DeleteSpecialty")]
        public async Task<IActionResult> DeleteSpecialty(IdDto idDto)
        {
            var dept = await _db.Departments.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (dept == null)
            {
                return NotFound($"specialty Id {idDto.Id} not exist");
            }
            _db.Departments.Remove(dept);
            _db.SaveChanges();
            return Ok(dept);

        }
        [HttpDelete("DeleteDoctor")]
        public async Task<IActionResult> DeleteDoctor(IdDto idDto)
        {
            var doctor = await _db.Doctors.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (doctor == null)
            {
                return NotFound($"doctor Id {idDto.Id} not exist");
            }
            _db.Doctors.Remove(doctor);
            _db.SaveChanges();
            return Ok(doctor);

        }
        [HttpDelete("DeleteRegion")]
        public async Task<IActionResult> DeleteRegion(IdDto idDto)
        {
            var region = await _db.Regions.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (region == null)
            {
                return NotFound($"region Id {idDto.Id} not exist");
            }
            _db.Regions.Remove(region);
            _db.SaveChanges();
            return Ok(region);

        }
        [HttpDelete("DeleteGovernorate")]
        public async Task<IActionResult> DeleteGovernorate(IdDto idDto)
        {
            var governorate = await _db.Governorates.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (governorate == null)
            {
                return NotFound($"governorate Id {idDto.Id} not exist");
            }
            _db.Governorates.Remove(governorate);
            _db.SaveChanges();
            return Ok(governorate);

        }

        [HttpPost("AddGovernorate")]
        public async Task<IActionResult> AddProduct(GovernorateDto governorateDto)
        {

            Governorate gov = new()
            {
               GovernorateName= governorateDto.GovernorateName,
            };
            await _db.Governorates.AddAsync(gov);
            _db.SaveChanges();
            return Ok(new { Message = "new governorate created successfully", Id = gov.Id });
        }

       [HttpPost("AddRegion")]
        public async Task<IActionResult> AddRegion(RegionDto regionDto)
        {


            var hisGov = await _db.Governorates.SingleOrDefaultAsync(x => x.Id == regionDto.GovernorateId);
            if (hisGov == null)
            {
                return NotFound($"governorate Id {regionDto.GovernorateId } not exist");
            }
            Region reg = new()
            {
                RegionName = regionDto.RegionName,
                GovernorateId = regionDto.GovernorateId,
            };
            await _db.Regions.AddAsync(reg);
            _db.SaveChanges();
            return Ok(new { Message = "new region created successfully", Id = reg.Id });
        }

        [HttpPost("AddSpecialty")]
        public async Task<IActionResult> AddSpecialty(SpecialtyDto specialtyDto)
        {
            Department department = new()
            {
                DepartmentName = specialtyDto.SpecialtyName,
            };
            await _db.Departments.AddAsync(department);
            _db.SaveChanges();
            return Ok(new { Message = "new Specialty created successfully", Id = department.Id });
        }



        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _db.Appointments.ToListAsync();
            var doctors = await _db.Doctors.ToListAsync();
            var patients = await _db.Patients.ToListAsync();
            var regions = await _db.Regions.ToListAsync();
            var list = new List<AppointmentDto>();

            foreach (var appointment in appointments)
            {
                var currentDoctortId = appointment.DoctorId;
                var currentPaientId = appointment.PatientId;

                var patientName = "not defiened";
                foreach (var patient in patients)
                {
                    
                    if (patient.Id == currentPaientId)
                    { 
                         patientName= patient.Name;
                    }
                }
                
                foreach (var doctor in doctors)
                {
                    if (doctor.Id == currentDoctortId)
                    {
                        var currentRegion="";
                        foreach (var R in regions)
                        {
                            if (R.Id== doctor.RegionId) { currentRegion = R.RegionName; }
                        }

                        AppointmentDto appointmentDto = new AppointmentDto()
                        {
                           AppointmentId = appointment.Id ,
                             PatientId= currentPaientId,
                             PatientName = patientName,
                             DoctorId = currentDoctortId,
                             DoctorName = doctor.Name,
                             Region = currentRegion,

                        };
                        list.Add(appointmentDto);
                    }
                }


            }
            return Ok(list);
        }
        [HttpGet("GetAllGoverorates")]
        public async Task<IActionResult> GetAllGoverorates()
        {
            var goverorates = await _db.Governorates.ToListAsync();
            
            var list = new List<GovernorateDto>();

            foreach (var goverorate in goverorates)
            {

            GovernorateDto governorateDto = new GovernorateDto()
                {
                    GovernorateId = goverorate.Id ,
                    GovernorateName = goverorate.GovernorateName,
                };
            list.Add(governorateDto);
                  
            }
            return Ok(list);
        }

        [HttpGet("GetAllRegions")]
        public async Task<IActionResult> GetAllRegions()
        {
            var regions = await _db.Regions.ToListAsync();
            var governorates = await _db.Governorates.ToListAsync();

            var list = new List<RegionDto>();

            foreach (var region in regions)
            {
                var currentGovernorateId =  region.GovernorateId;
                var currentGovernorateName = "";
                foreach (var governorate in governorates)
                {
                    if (currentGovernorateId== governorate.Id) { currentGovernorateName = governorate.GovernorateName; }
                }
                RegionDto regionDto = new RegionDto()
                {
                    RegionId = region.Id ,
                    RegionName = region.RegionName,
                    GovernorateId= region.GovernorateId,
                    GovernorateName= currentGovernorateName,
                };
                list.Add(regionDto);

            }
            return Ok(list);
        }

        [HttpGet("GetAllSpecilties")]
        public async Task<IActionResult> GetAllSpecilties()
        {
            var specilties = await _db.Departments.ToListAsync();

            var list = new List<SpecialtyDto>();

            foreach (var specilty in specilties)
            {

                SpecialtyDto specialtyDto = new SpecialtyDto()
                {
                    Id = specilty.Id,
                    SpecialtyName = specilty.DepartmentName,
                };
                list.Add(specialtyDto);

            }
            return Ok(list);
        }

    }
}
