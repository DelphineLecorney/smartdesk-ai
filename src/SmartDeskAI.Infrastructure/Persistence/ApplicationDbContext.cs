using Microsoft.EntityFrameworkCore;
using SmartDeskAI.Application.Common.Interfaces;
using SmartDeskAI.Domain.Entities;

namespace SmartDeskAI.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenantService _currentTenant;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenant)
        : base(options)
    {
        _currentTenant = currentTenant;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Subdomain).IsRequired().HasMaxLength(100);
            builder.HasIndex(t => t.Subdomain).IsUnique();
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);

            // Email est un Value Object, mappé comme propriété "owned",
            // stocké dans une colonne simple sans exposer sa structure interne.
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value).HasColumnName("Email").IsRequired().HasMaxLength(320);
            });

            builder.Property(u => u.TenantId).IsRequired();

            // Filtre appliqué automatiquement à toute requête LINQ sur User.
            builder.HasQueryFilter(u => u.TenantId == _currentTenant.TenantId);

            // Unicité de l'email par tenant (deux tenants différents peuvent
            // avoir un utilisateur avec le même email, ce sont des comptes distincts).
            builder.HasIndex("TenantId", "Email").IsUnique();
        });
    }
}