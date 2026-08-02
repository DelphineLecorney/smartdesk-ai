using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartDeskAI.Application.Common.Interfaces;
using SmartDeskAI.Infrastructure.Persistence;
using SmartDeskAI.Infrastructure.Persistence.Repositories;
using SmartDeskAI.Infrastructure.Services;

namespace SmartDeskAI.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Enregistre les services d'infrastructure (EF Core, Repositories, Services externes) dans la collection de services.
    /// </summary>
    /// <param name="services">La collection de services de l'application.</param>
    /// <param name="configuration">La configuration de l'application pour l'accès aux chaînes de connexion.</param>
    /// <returns>La collection de services mise à jour pour le chaînage.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}