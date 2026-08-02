using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SmartDeskAI.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Enregistre les services applicatifs (MediatR, Handlers, FluentValidation) dans la collection de services.
    /// </summary>
    /// <param name="services">La collection de services de l'application.</param>
    /// <returns>La collection de services mise à jour pour le chaînage.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}