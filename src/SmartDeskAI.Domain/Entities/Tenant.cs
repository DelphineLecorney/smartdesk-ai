using SmartDeskAI.Domain.Common;

namespace SmartDeskAI.Domain.Entities
{
    /// <summary>
    /// Représente un Tenant (entreprise cliente) au sein du système Saas multi-tenant.
    /// Racine d'agrégat pour la gestion des comptes clients et de leurs abonnements.
    /// </summary>
    public sealed class Tenant : BaseEntity
    {
        public string Name { get; private set; }
        public string Subdomain { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string SubscriptionPlan { get; private set; }

        private Tenant() 
        {
            Name = string.Empty;
            Subdomain = string.Empty;
            SubscriptionPlan = string.Empty;
        }

        private Tenant(string name, string subdomain, string subscriptionPlan)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentException("Le nom du tenant est obligatoire", nameof(name));

            if (string.IsNullOrWhiteSpace(subdomain))
                throw new ArgumentException("Le sous-domaine du tenant est obligatoire.", nameof(subdomain));

            Name = name.Trim();
            Subdomain = subdomain.Trim().ToLowerInvariant();
            SubscriptionPlan = subscriptionPlan;
            CreatedAt = DateTime.UtcNow;            
        }

        public static Tenant Create(string name, string subdomain, string subscriptionPlan = "Free")
            => new Tenant(name, subdomain, subscriptionPlan);
    }
}
