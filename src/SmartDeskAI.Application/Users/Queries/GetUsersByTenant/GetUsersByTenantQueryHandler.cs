using MediatR;
using SmartDeskAI.Application.Common.Interfaces;
using SmartDeskAI.Application.Users.Queries.Common;

namespace SmartDeskAI.Application.Users.Queries.GetUsersByTenant;

/// <summary>
/// Gestionnaire de la requête de récupération de la liste des utilisateurs d'un Tenant.
/// </summary>
public sealed class GetUsersByTenantQueryHandler : IRequestHandler<GetUsersByTenantQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersByTenantQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetUsersByTenantQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users
            .Select(u => new UserDto(u.Id, u.Email.Value, u.Role.ToString(), u.Status.ToString(), u.CreatedAt))
            .ToList();
    }
}
