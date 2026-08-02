using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartDeskAI.Infrastructure.Persistence;

/// <summary>
/// Utilisée uniquement par les outils EF Core (dotnet ef migrations add/update)
/// en ligne de commande où aucun contexte HTTP n'existe pour résoudre
/// ICurrentTenantService normalement. On fournit ici une valeur factice,
/// elle n'affecte jamais le comportement réel de l'application (seulement
/// la génération du schéma de migration).
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private sealed class DesignTimeTenantService : Application.Common.Interfaces.ICurrentTenantService
    {
        public Guid TenantId => Guid.Empty;
        public bool IsAuthenticated => false;
    }

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost;Database=SmartDeskAI;Trusted_Connection=True;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options, new DesignTimeTenantService());
    }
}