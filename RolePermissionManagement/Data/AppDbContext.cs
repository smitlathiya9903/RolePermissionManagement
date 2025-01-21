using Microsoft.EntityFrameworkCore;
using RolePermissionManagement.Models.Entities;

namespace RolePermissionManagement.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Permission
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(p => p.Id); // Primary Key
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            });

            // Configure Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id); // Primary Key
                entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id); // Primary Key
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);

                // One-to-Many: Role → Users
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
            });

            // Configure Many-to-Many: Role ↔ Permission
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission", // Name of the join table
                    r => r.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                    p => p.HasOne<Role>().WithMany().HasForeignKey("RoleId")
                );

            // Seed initial data for Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Commission", Active = true },
                new Permission { Id = 2, Name = "Endorsements", Active = true },
                new Permission { Id = 3, Name = "Risk Management", Active = true },
                new Permission { Id = 4, Name = "Administration", Active = true },
                new Permission { Id = 5, Name = "Utilities", Active = true },
                new Permission { Id = 6, Name = "Inactive", Active = false }
            );
        }
    }
}
