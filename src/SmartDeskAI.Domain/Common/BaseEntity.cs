namespace SmartDeskAI.Domain.Common
{
    /// <summary>
    /// Classe de base abstraite pour toutes les entités du domaine (DDD).
    /// Elle centralise la gestion de l'identité et le système de notification d'évènements
    /// pour découpler la logique métier de ses effets de bord (ex: envoi d'emails, requêtes IA).
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        private readonly List<object> _domainEvents = new();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
