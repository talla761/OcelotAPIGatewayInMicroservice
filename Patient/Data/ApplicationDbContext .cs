using Microsoft.EntityFrameworkCore;

namespace Patient.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient.Models.Patient> Patients { get; set; }
    }
}
