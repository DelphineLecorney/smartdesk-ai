using SmartDeskAI.Domain.Common;
using SmartDeskAI.Domain.Enums;
using SmartDeskAI.Domain.Exceptions;
using SmartDeskAI.Domain.ValueObjects;

namespace SmartDeskAI.Domain.Entities
{
    /// <summary>
    /// Représente un utilisateur (Agent, Admin ou Client) rattaché à un Tenant.
    /// Gère le cycle de vie du compte (Invitation, Activation, Désactivation) et contrôle les droits d'accès.
    /// </summary>
    public sealed class User : TenantOwnedEntity
    {
        public Email Email { get; private set; }
        public UserRole Role { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ActivatedAt { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }

#pragma warning disable CS8618
        private User() : base(Guid.NewGuid()) { }
#pragma warning restore CS8618

        private User(Guid tenantId, Email email, UserRole role) : base(tenantId)
        {
            Email = email;
            Role = role;
            Status = UserStatus.Invited;
            CreatedAt = DateTime.UtcNow;
        }

        public static User Invite(Guid tenantId, string rawEmail, UserRole role)
            => new(tenantId, Email.Create(rawEmail), role);

        public void Activate()
        {
            if (Status != UserStatus.Invited)
                throw new InvalidUserStateTransitionException(Status.ToString(), UserStatus.Active.ToString());

            Status = UserStatus.Active;
            ActivatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (Status == UserStatus.Deactivated)
                throw new InvalidUserStateTransitionException(Status.ToString(), UserStatus.Deactivated.ToString());

            Status = UserStatus.Deactivated;
            DeactivatedAt = DateTime.UtcNow;

            AddDomainEvent(new Events.UserDeactivatedEvent(Id, TenantId));
        }             
    }
}