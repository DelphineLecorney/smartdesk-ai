namespace SmartDeskAI.Domain.Enums
{
    /// <summary>
    /// Définit les rôles des utilisateurs au sein du système.
    /// Détermine le niveau d'accès et les permissions sur les fonctionnalités de l'application.
    /// </summary>
    public enum UserRole
    {
        ClientFinal = 0,
        Agent = 1,
        AdministrateurClient = 2
    }
}
