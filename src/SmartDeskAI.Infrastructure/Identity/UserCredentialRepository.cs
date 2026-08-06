using Microsoft.EntityFrameworkCore;
using SmartDeskAI.Application.Common.Interfaces;
using SmartDeskAI.Infrastructure.Persistence;

namespace SmartDeskAI.Infrastructure.Identity;

/// <summary>
/// Implémentation EF Core du dépôt d'identifiants utilisateurs.
/// Gère la lecture et l'écriture des empreintes (hashes) de mots de passe dans la base de données.
/// </summary>
public sealed class UserCredentialRepository : IUserCredentialRepository
{
    private readonly ApplicationDbContext _context;

    public UserCredentialRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetPasswordHashByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var credential = await _context.UserCredentials
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        return credential?.PasswordHash;
    }

    public async Task CreateAsync(Guid userId, string passwordHash, CancellationToken cancellationToken)
    {
        var credential = UserCredential.Create(userId, passwordHash);
        await _context.UserCredentials.AddAsync(credential, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}