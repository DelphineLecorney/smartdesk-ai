using SmartDeskAI.Application.Common.Interfaces;

namespace SmartDeskAI.Infrastructure.Identity;

/// <summary>
/// Service de hachage et de vérification des mots de passe basé sur l'algorithme BCrypt.
/// </summary>
public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword) => BCrypt.Net.BCrypt.HashPassword(plainPassword);

    public bool Verify(string plainPassword, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
}