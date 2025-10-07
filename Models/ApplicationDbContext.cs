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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Product>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("DATETIME('now')"); // SQLite specific function for current timestamp
        modelBuilder.Entity<ProductDetails>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ProductDetails>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("DATETIME('now')");
        modelBuilder.Entity<Shipment>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Shipment>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("DATETIME('now')");
        modelBuilder.Entity<User>()
        .Property(e => e.Id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<User>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("DATETIME('now')");
    }
}