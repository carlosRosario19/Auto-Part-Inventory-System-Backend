using AutoPartInventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Data;

public partial class AutoPartInventoryDBContext : DbContext
{
    public AutoPartInventoryDBContext()
    {
    }

    public AutoPartInventoryDBContext(DbContextOptions<AutoPartInventoryDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AutoPart> AutoParts { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AutoPart>(entity =>
        {
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.AutoParts).HasConstraintName("FK_AutoParts_Categories");

            entity.HasMany(d => d.Brands).WithMany(p => p.AutoParts)
                .UsingEntity<Dictionary<string, object>>(
                    "AutoPartBrand",
                    r => r.HasOne<Brand>().WithMany()
                        .HasForeignKey("BrandId")
                        .HasConstraintName("FK_AutoPartBrands_Brands"),
                    l => l.HasOne<AutoPart>().WithMany()
                        .HasForeignKey("AutoPartId")
                        .HasConstraintName("FK_AutoPartBrands_Parts"),
                    j =>
                    {
                        j.HasKey("AutoPartId", "BrandId");
                        j.ToTable("AutoPartBrands");
                    });

            entity.HasMany(d => d.Vehicles).WithMany(p => p.AutoParts)
                .UsingEntity<Dictionary<string, object>>(
                    "AutoPartVehicle",
                    r => r.HasOne<Vehicle>().WithMany()
                        .HasForeignKey("VehicleId")
                        .HasConstraintName("FK_AutoPartVehicles_Vehicles"),
                    l => l.HasOne<AutoPart>().WithMany()
                        .HasForeignKey("AutoPartId")
                        .HasConstraintName("FK_AutoPartVehicles_Parts"),
                    j =>
                    {
                        j.HasKey("AutoPartId", "VehicleId");
                        j.ToTable("AutoPartVehicles");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRoles_Roles"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRoles_Users"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                    });
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasOne(d => d.Brand).WithMany(p => p.Vehicles).HasConstraintName("FK_Vehicles_Brands");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
