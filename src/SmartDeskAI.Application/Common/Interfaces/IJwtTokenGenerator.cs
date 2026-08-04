using SmartDeskAI.Domain.Entities;

namespace SmartDeskAI.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    /// <summary>
    /// Génère un JWT contenant, entre autres, les claims "sub" (UserId),
    /// "tenant_id" et "role", c'est précisément ce claim "tenant_id"
    /// que CurrentTenantService ira lire à chaque requête.
    /// </summary>
    string GenerateToken(User user);
}