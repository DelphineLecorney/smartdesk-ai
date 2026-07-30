using MediatR;
using SmartDeskAI.Application.Users.Queries.Common;

namespace SmartDeskAI.Application.Users.Queries.GetUsersByTenant;

/// <summary>
/// Requête CQRS permettant de récupérer l'ensemble des utilisateurs appartenant au Tenant courant.
/// </summary>
/// <returns>Une liste d'objets <see cref="UserDto"/>.</returns>
public sealed record GetUsersByTenantQuery : IRequest<List<UserDto>>;