using Microsoft.EntityFrameworkCore;
using TheStreets_BE.Models;

namespace TheStreets_BE.Data
{
    public class TheStreetsDbContext : DbContext
    {
        public TheStreetsDbContext(DbContextOptions<TheStreetsDbContext> options)
            : base(options) { }

        public DbSet<BlogPost> Posts => Set<BlogPost>();
        public DbSet<PostLike> Likes => Set<PostLike>();
        public DbSet<PostComment> Comments => Set<PostComment>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<BlogPost>(e =>
            {
                e.Property(p => p.Message).IsRequired().HasMaxLength(2000);
                e.Property(p => p.CreatedByUserId).IsRequired().HasMaxLength(128);
                e.Property(p => p.CreatedByDisplayName).HasMaxLength(256);

                // Timestamps: defaults and generated patterns
                e.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")    // SQLite UTC timestamp
                    .ValueGeneratedOnAdd();

                e.Property(p => p.UpdatedAt)
                    .IsRequired(false);

                e.HasMany(p => p.Likes)
                    .WithOne(l => l.Post)
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(p => p.Comments)
                    .WithOne(c => c.Post)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<PostLike>(e =>
            {
                e.HasIndex(l => new { l.PostId, l.UserId }).IsUnique(); // one like per user per post
                e.Property(l => l.UserId).IsRequired().HasMaxLength(128);
            });

            b.Entity<PostComment>(e =>
            {
                e.Property(c => c.UserId).IsRequired().HasMaxLength(128);
                e.Property(c => c.UserDisplayName).HasMaxLength(256);
                e.Property(c => c.Message).IsRequired().HasMaxLength(2000);
            });
        }
    }
}