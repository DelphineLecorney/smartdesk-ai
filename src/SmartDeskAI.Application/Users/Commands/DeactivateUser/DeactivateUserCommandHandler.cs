using MediatR;
using SmartDeskAI.Application.Common.Interfaces;

namespace SmartDeskAI.Application.Users.Commands.DeactivateUser
{
    /// <summary>
    /// Gestionnaire de la commande de désactivation d'un compte utilisateur.
    /// Orchestre la récupération de l'entité, l'exécution de la règle métier de désactivation
    /// et la persistance du changement d'état.
    /// </summary>
    public sealed class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeactivateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw new KeyNotFoundException($"Utilisateur {request.UserId} introuvable.");

            user.Deactivate();

            await _userRepository.SaveChangeAsync(cancellationToken);
        }
    }
}
