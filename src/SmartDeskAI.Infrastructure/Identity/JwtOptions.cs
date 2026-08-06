namespace SmartDeskAI.Infrastructure.Identity;

/// <summary>
/// Options de configuration fortement typées pour la génération et la validation des jetons JWT.
/// Mappées depuis la section "Jwt" du fichier appsettings.json.
/// </summary>
public sealed class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}