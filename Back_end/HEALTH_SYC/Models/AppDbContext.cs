
using Microsoft.EntityFrameworkCore;

namespace HEALTH_SYC.Models
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Symptom_of_disease> Symptom_of_diseases { get; set; }
        public DbSet<Disease_of_patient> Disease_of_patients { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<Complain> Complains { get; set; }
        public DbSet<Booking> Bookings { get; set; }

    }
}
