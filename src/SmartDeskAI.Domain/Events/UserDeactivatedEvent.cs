namespace SmartDeskAI.Domain.Events;

/// <summary>
/// Levé quand un utilisateur est désactivé. Le Module B (Ticketing) s'y abonnera
/// pour réassigner les tickets en cours vers la file d'attente non assignée
/// (règle validée dans le CDCF) sans que User ait besoin de connaître Ticket.
/// </summary>
public sealed record UserDeactivatedEvent(Guid UserId, Guid TenantId);