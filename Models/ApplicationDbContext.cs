using Microsoft.EntityFrameworkCore;

namespace Bonjour.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ProductDetails> ProductDetails { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserHasRole> UserHasRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RoleHasPermission> RoleHasPermissions { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ShipmentProduct> ShipmentProducts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Product>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Product>()
        .Property(e => e.UpdatedAt)
        .HasColumnType("timestamp");
        modelBuilder.Entity<ProductDetails>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ProductDetails>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<ProductDetails>()
        .Property(e => e.UpdatedAt)
        .HasColumnType("timestamp");
        modelBuilder.Entity<Shipment>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Shipment>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Shipment>()
        .Property(e => e.UpdatedAt)
        .HasColumnType("timestamp");
        modelBuilder.Entity<User>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<User>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<User>()
        .Property(e => e.UpdatedAt)
        .HasColumnType("timestamp");
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Admin", Username = "admin", Email = "admin@email.com", Password = "PC+r5h/39f33U0dRi4bUZNUlDxlHbRWpXX5L0hZGhGQ=", Salt = "QGkIA+8LmTIoNvcvVmrH1A==" }
            );
        modelBuilder.Entity<Role>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Role>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Role>()
        .Property(e => e.UpdatedAt)
        .HasColumnType("timestamp");
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" },
            new Role { Id = 3, Name = "Loading" },
            new Role { Id = 4, Name = "Unloading" }
            );
        modelBuilder.Entity<UserHasRole>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<UserHasRole>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Permission>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Permission>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<RoleHasPermission>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<RoleHasPermission>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Contract>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Contract>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Contract>()
        .Property(e => e.UpdatedAt)
        .HasColumnType("timestamp");
        modelBuilder.Entity<ShipmentProduct>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ShipmentProduct>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<ShipmentProduct>()
        .Property(e => e.UpdatedAt)
        .HasColumnType("timestamp");
    }
}