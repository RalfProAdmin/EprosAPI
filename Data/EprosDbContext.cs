using Epros_CareerHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Epros_CareerHubAPI.Data
{
    public class EprosDbContext : DbContext
    {
        public EprosDbContext(DbContextOptions<EprosDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Added JobPostings
        public DbSet<JobPosting> JobPostings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map JobPosting defaults/constraints optionally (kept minimal)
            modelBuilder.Entity<JobPosting>(b =>
            {
                b.HasKey(j => j.JobId);
                b.Property(j => j.Title).HasMaxLength(200).IsRequired();
                b.Property(j => j.Department).HasMaxLength(200).IsRequired(false);
                b.Property(j => j.DescriptionHtml).HasColumnType("nvarchar(max)").IsRequired(false);
                b.Property(j => j.SalaryRange).HasMaxLength(100).IsRequired(false);
                b.Property(j => j.CreatedAt).HasDefaultValueSql("getdate()");
                b.Property(j => j.MinExperiences).HasDefaultValue(0);
                b.Property(j => j.IsActive).HasDefaultValue(true);

                // optional relationship
                b.HasOne(j => j.PostedByUser)
                 .WithMany()
                 .HasForeignKey(j => j.PostedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
