using MediatR;

namespace SmartDeskAI.Application.Users.Commands.ActivateUser;

/// <summary>
/// Commande CQRS représentant l'intention d'activer le compte d'un utilisateur invité.
/// </summary>
/// <param name="UserId">L'identifiant unique (Guid) de l'utilisateur à activer.</param>
public sealed record ActivateUserCommand(Guid UserId) : IRequest;