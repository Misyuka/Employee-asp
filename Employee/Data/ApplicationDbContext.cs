using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Employee.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int Role { get; set; }
        public int DM { get; set; }
    }
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{

        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<EmployeeInfo> Employees { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionStringCasa = "Server=(localdb)\\mssqllocaldb;Database=aspnet-Employee-ed251413-fdfb-4201-8df5-2890c7c33ea7;Trusted_Connection=True;MultipleActiveResultSets=true";
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionStringCasa);
            }
        }
    }
}