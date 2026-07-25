namespace SmartDeskAI.Domain.Enums
{
    /// <summary>
    /// Définit les différents états du cycle de vie d'un compte utilisateur dans le système.
    /// Permet de contrôler l'accès à la plateforme et de gérer le processus d'accueil (onboarding).
    /// </summary>
    public enum UserStatus
    {
        Invited = 0,
        Active = 1,
        Deactivated = 2
    }
}
