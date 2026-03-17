using Microsoft.EntityFrameworkCore;
using TheStreets_BE.Models;

namespace TheStreets_BE.Data
{
    public class TheStreetsDbContext : DbContext
    {
        public TheStreetsDbContext(DbContextOptions<TheStreetsDbContext> options)
            : base(options) { }

        public DbSet<BlogPost> Posts => Set<BlogPost>();
    }
}