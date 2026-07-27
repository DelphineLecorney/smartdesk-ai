using MediatR;
using SmartDeskAI.Application.Common.Interfaces;

namespace SmartDeskAI.Application.Users.Commands.ActivateUser;

public sealed class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Utilisateur {request.UserId} introuvable.");

        user.Activate();

        await _userRepository.SaveChangeAsync(cancellationToken);
    }
}