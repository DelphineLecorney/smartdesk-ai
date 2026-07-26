namespace SmartDeskAI.Application.Common.Interfaces;

/// <summary>
/// Abstraction du "tenant courant". L'implémentation concrète (Infrastructure)
/// lira cette valeur depuis les claims du token authentifié, jamais depuis
/// un paramètre de requête HTTP. C'est le point central qui garantit
/// l'isolation multi-tenant.
/// </summary>
public interface ICurrentTenantService
{
    Guid TenantId { get; }
    bool IsAuthenticated { get; }
}
