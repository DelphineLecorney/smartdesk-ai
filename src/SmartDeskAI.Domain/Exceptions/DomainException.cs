namespace SmartDeskAI.Domain.Exceptions
{
    /// <summary>
    /// Exception de base pour  les violations de règles métier du domaine.
    /// Permet de distinguer les erreurs de logique métier des erreurs purement techniques, d'infrastructure.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public sealed class InvalidUserStateTransitionException : DomainException
    {
        public InvalidUserStateTransitionException(string from, string to)
            : base($"Transition invalide : impossible de passer du statut '{from}' au statut '{to}'.") { }
    }
}
