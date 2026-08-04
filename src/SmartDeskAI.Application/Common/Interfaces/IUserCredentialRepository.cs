namespace SmartDeskAI.Application.Common.Interfaces;

/// <summary>
/// Contrat de dépôt dédié à la gestion de la sécurité et des identifiants (mots de passe hachés) des utilisateurs.
/// </summary>
public interface IUserCredentialRepository
{
    Task<string?> GetPasswordHashByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task CreateAsync(Guid userId, string passwordHash, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}