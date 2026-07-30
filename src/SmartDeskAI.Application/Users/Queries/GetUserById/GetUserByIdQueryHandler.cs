using MediatR;
using SmartDeskAI.Application.Common.Interfaces;
using SmartDeskAI.Application.Users.Queries.Common;

namespace SmartDeskAI.Application.Users.Queries.GetUserById;

/// <summary>
/// Gestionnaire de la requête de récupération d'un utilisateur par son identifiant.
/// Interroge le dépôt et mappe l'entité du Domaine vers un DTO d'exposition.
/// </summary>
public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        return user is null
            ? null
            : new UserDto(user.Id, user.Email.Value, user.Role.ToString(), user.Status.ToString(), user.CreatedAt);
    }
}