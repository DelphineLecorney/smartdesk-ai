using MediatR;

namespace SmartDeskAI.Application.Users.Commands.InviteUser;

/// <summary>
/// Pas de TenantId dans cette Command.
/// Il sera résolu côté Handler via ICurrentTenantService, jamais fourni par l'appelant.
/// Un DTO qui accepterait un TenantId depuis le client serait une faille d'isolation.
/// </summary>
public sealed record InviteUserCommand(string Email, Domain.Enums.UserRole Role) : IRequest<Guid>;