using Microsoft.EntityFrameworkCore;
using MVCTest1.Models;

namespace MVCTest1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<general_user> general_user { get; set; }

    }
}
