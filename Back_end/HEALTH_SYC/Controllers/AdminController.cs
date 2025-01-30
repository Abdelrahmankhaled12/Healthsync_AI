using HEALTH_SYC.DTOs;
using HEALTH_SYC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace HEALTH_SYC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure the user is logged in
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // Delete specialty
        [HttpDelete("DeleteSpecialty")]
        public async Task<IActionResult> DeleteSpecialty(IdDto idDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var dept = await _db.Departments.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (dept == null)
            {
                return NotFound($"Specialty Id {idDto.Id} does not exist.");
            }
            _db.Departments.Remove(dept);
            await _db.SaveChangesAsync();
            return Ok(dept);
        }

        // Delete doctor
        [HttpDelete("DeleteDoctor")]
        public async Task<IActionResult> DeleteDoctor(IdDto idDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var doctor = await _db.Doctors.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (doctor == null)
            {
                return NotFound($"Doctor Id {idDto.Id} does not exist.");
            }
            _db.Doctors.Remove(doctor);
            await _db.SaveChangesAsync();
            return Ok(doctor);
        }

        // Delete region
        [HttpDelete("DeleteRegion")]
        public async Task<IActionResult> DeleteRegion(IdDto idDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var region = await _db.Regions.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (region == null)
            {
                return NotFound($"Region Id {idDto.Id} does not exist.");
            }
            _db.Regions.Remove(region);
            await _db.SaveChangesAsync();
            return Ok(region);
        }

        // Delete governorate
        [HttpDelete("DeleteGovernorate")]
        public async Task<IActionResult> DeleteGovernorate(IdDto idDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var governorate = await _db.Governorates.SingleOrDefaultAsync(x => x.Id == idDto.Id);
            if (governorate == null)
            {
                return NotFound($"Governorate Id {idDto.Id} does not exist.");
            }
            _db.Governorates.Remove(governorate);
            await _db.SaveChangesAsync();
            return Ok(governorate);
        }

        // Add governorate
        [HttpPost("AddGovernorate")]
        public async Task<IActionResult> AddGovernorate(GovernorateDto governorateDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            Governorate gov = new()
            {
                GovernorateName = governorateDto.GovernorateName,
            };
            await _db.Governorates.AddAsync(gov);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "New governorate added successfully", Id = gov.Id });
        }

        // Add region
        [HttpPost("AddRegion")]
        public async Task<IActionResult> AddRegion(RegionDto regionDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var hisGov = await _db.Governorates.SingleOrDefaultAsync(x => x.Id == regionDto.GovernorateId);
            if (hisGov == null)
            {
                return NotFound($"Governorate Id {regionDto.GovernorateId} does not exist.");
            }

            Region reg = new()
            {
                RegionName = regionDto.RegionName,
                GovernorateId = regionDto.GovernorateId,
            };
            await _db.Regions.AddAsync(reg);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "New region added successfully", Id = reg.Id });
        }

        // Add specialty
        [HttpPost("AddSpecialty")]
        public async Task<IActionResult> AddSpecialty(SpecialtyDto specialtyDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            Department department = new()
            {
                DepartmentName = specialtyDto.SpecialtyName,
            };
            await _db.Departments.AddAsync(department);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "New specialty added successfully", Id = department.Id });
        }

        // Get all appointments
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

                var patientName = "not defined";
                foreach (var patient in patients)
                {
                    if (patient.Id == currentPaientId)
                    {
                        patientName = patient.Name;
                    }
                }

                foreach (var doctor in doctors)
                {
                    if (doctor.Id == currentDoctortId)
                    {
                        var currentRegion = "";
                        foreach (var R in regions)
                        {
                            if (R.Id == doctor.RegionId) { currentRegion = R.RegionName; }
                        }

                        AppointmentDto appointmentDto = new AppointmentDto()
                        {
                            AppointmentId = appointment.Id,
                            PatientId = currentPaientId,
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

        // Get all regions
        [HttpGet("GetAllRegions")]
        public async Task<IActionResult> GetAllRegions()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var regions = await _db.Regions.ToListAsync();
            var list = new List<RegionDto>();

            foreach (var region in regions)
            {
                RegionDto regionDto = new RegionDto()
                {
                    RegionId = region.Id,
                    RegionName = region.RegionName,
                    GovernorateId = region.GovernorateId
                };
                list.Add(regionDto);
            }

            return Ok(list);
        }

        // Get all specialties
        [HttpGet("GetAllSpecialties")]
        public async Task<IActionResult> GetAllSpecialties()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var specialties = await _db.Departments.ToListAsync();
            var list = new List<SpecialtyDto>();

            foreach (var specialty in specialties)
            {
                SpecialtyDto specialtyDto = new SpecialtyDto()
                {
                    Id = specialty.Id,
                    SpecialtyName = specialty.DepartmentName
                };
                list.Add(specialtyDto);
            }

            return Ok(list);
        }

        // Get all patients
        [HttpGet("GetAllPatients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Unauthorized("You don't have the required permissions.");
            }

            var patients = await _db.Patients.ToListAsync();
            var list = new List<PatientDto>();

            foreach (var patient in patients)
            {
                PatientDto patientDto = new PatientDto()
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Phone = patient.Phone,
                    Address = patient.Address,
                    Age = patient.Age
                };
                list.Add(patientDto);
            }

            return Ok(list);
        }
        [HttpGet("GetAllGovernorates")]
        public async Task<IActionResult> GetAllGovernorates()
        {
            var governorates = await _db.Governorates.ToListAsync();

            var list = new List<GovernorateDto>();

            foreach (var governorate in governorates)
            {
                GovernorateDto governorateDto = new GovernorateDto()
                {
                    GovernorateId = governorate.Id,
                    GovernorateName = governorate.GovernorateName,
                };
                list.Add(governorateDto);
            }

            if (list.Count == 0)
            {
                return NotFound("No governorates found.");
            }

            return Ok(list);
        }

    }
}
