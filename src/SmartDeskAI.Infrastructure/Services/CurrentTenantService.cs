using Microsoft.AspNetCore.Http;
using SmartDeskAI.Application.Common.Interfaces;

namespace SmartDeskAI.Infrastructure.Services;

/// <summary>
/// Service d'infrastructure fournissant le contexte du Tenant pour la requête HTTP courante.
/// Extrait les informations à partir des claims du jeton d'authentification de l'utilisateur.
/// </summary>
public sealed class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid TenantId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");

            if (claim is null || !Guid.TryParse(claim.Value, out var tenantId))
                throw new UnauthorizedAccessException("Aucun TenantId valide trouvé dans le contexte d'authentification.");

            return tenantId;
        }
    }
}