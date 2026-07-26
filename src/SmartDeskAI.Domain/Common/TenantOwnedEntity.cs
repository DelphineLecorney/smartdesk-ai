namespace SmartDeskAI.Domain.Common
{
    /// <summary>
    /// Classe de base pour toutes les entités du domaine appartenant à un Tenant (Saas multi-tenant)
    /// Elle assure que l'isolation des données est garantie dès la création de l'objet en mémoire
    /// et fournit le point d'ancrage pour les filtres d'isolation globaux EF COre.
    /// </summary>
    public abstract class TenantOwnedEntity : BaseEntity
    {
        public Guid TenantId { get; protected set; }

        protected TenantOwnedEntity(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("Le TenantId ne peut pas être vide", nameof(tenantId));

            TenantId = tenantId;
        }
    }
}
