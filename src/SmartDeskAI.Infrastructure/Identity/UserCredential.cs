namespace SmartDeskAI.Infrastructure.Identity;

/// <summary>
/// Entité purement technique, volontairement absente du Domain (voir ADR-0007).
/// Ne porte aucune règle métier, juste le lien vers l'utilisateur et son mot de passe hashé.
/// </summary>
public sealed class UserCredential
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private UserCredential(Guid userId, string passwordHash)
    {
        UserId = userId;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

#pragma warning disable CS8618
    private UserCredential() { }
#pragma warning restore CS8618

    public static UserCredential Create(Guid userId, string passwordHash)
        => new(userId, passwordHash);

    public void UpdatePasswordHash(string newPasswordHash) => PasswordHash = newPasswordHash;
}