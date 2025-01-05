using ArgonBot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArgonBot
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new UserConfiguration().Configure(modelBuilder.Entity<User>());
        }
    }
}
