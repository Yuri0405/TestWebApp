using Microsoft.EntityFrameworkCore;

namespace TestWebApp.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : 
            base(options) 
        { 
        }

        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
            .Property(p => p.Salary)
            .HasPrecision(18, 2);
        }
    }
}
