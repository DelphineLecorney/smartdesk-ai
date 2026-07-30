using MediatR;
using SmartDeskAI.Application.Users.Queries.Common;

namespace SmartDeskAI.Application.Users.Queries.GetUserById;

/// <summary>
/// Requête CQRS permettant de récupérer les informations d'un utilisateur via son identifiant unique.
/// </summary>
/// <param name="UserId">L'identifiant unique (<see cref="Guid"/>) de l'utilisateur recherché.</param>
/// <returns>Un objet <see cref="UserDto"/> si l'utilisateur existe, sinon <c>null</c>.</returns>
public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;