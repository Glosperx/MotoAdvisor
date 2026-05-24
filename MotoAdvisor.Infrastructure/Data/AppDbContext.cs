using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;

namespace MotoAdvisor.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<MotorcycleImage> MotorcycleImages => Set<MotorcycleImage>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserFavorite>()
            .HasKey(uf => new { uf.UserId, uf.MotorcycleId });

        builder.Entity<UserFavorite>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(uf => uf.UserId);

        builder.Entity<UserFavorite>()
            .HasOne(uf => uf.Motorcycle)
            .WithMany(m => m.FavoritedBy)
            .HasForeignKey(uf => uf.MotorcycleId);

        builder.Entity<Motorcycle>()
            .Property(m => m.Price)
            .HasColumnType("decimal(18,2)");

        // Store embedding vector as JSON column
        builder.Entity<Motorcycle>()
            .Property(m => m.EmbeddingVector)
            .HasColumnType("jsonb");
    }
}
