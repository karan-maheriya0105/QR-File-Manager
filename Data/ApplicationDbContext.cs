using BrochureAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BrochureAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ClientForm> ClientForms { get; set; }
        public DbSet<SmtpSettings> SmtpSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add unique constraint for User.StrName
            modelBuilder.Entity<User>()
                .HasIndex(u => u.StrName)
                .IsUnique();

            // Add unique constraint for User.StrEmailId
            modelBuilder.Entity<User>()
                .HasIndex(u => u.StrEmailId)
                .IsUnique();

            // Add unique constraint for Category.StrCategory
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.StrCategory)
                .IsUnique();

            // Configure relationships
            modelBuilder.Entity<ClientForm>()
                .HasOne(cf => cf.Category)
                .WithMany()
                .HasForeignKey(cf => cf.StrCategoryGUID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 