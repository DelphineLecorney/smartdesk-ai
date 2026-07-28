using MediatR;

namespace SmartDeskAI.Application.Users.Commands.DeactivateUser;

/// <summary>
/// Commande CQRS représentant l'intention de désactiver un compte utilisateur.
/// </summary>
/// <param name="UserId">L'identifiant unique (Guid) de l'utilisateur à désactiver.</param>

public sealed record DeactivateUserCommand(Guid UserId) : IRequest;