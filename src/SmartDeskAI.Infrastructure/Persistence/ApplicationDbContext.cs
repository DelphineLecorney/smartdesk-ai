using Microsoft.EntityFrameworkCore;
using SmartDeskAI.Application.Common.Interfaces;
using SmartDeskAI.Domain.Entities;
using SmartDeskAI.Domain.ValueObjects;

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

            // Email reste un Value Object côté Domain mais est mappé comme une simple
            // colonne string convertie (pas comme un type "owned" séparé) car EF Core 10
            // ne supporte pas bien les index composites qui mélangent une propriété
            // owned et une propriété du propriétaire (limitation levée seulement en EF Core 11).
            builder.Property(u => u.Email)
                .HasConversion(email => email.Value, value => Email.Create(value))
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(320);

            builder.Property(u => u.TenantId).IsRequired();

            builder.HasQueryFilter(u => u.TenantId == _currentTenant.TenantId);

            builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
        });
    }
}