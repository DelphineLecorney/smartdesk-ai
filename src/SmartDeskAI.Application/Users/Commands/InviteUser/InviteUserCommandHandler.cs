using SmartDeskAI.Application.Common.Interfaces;
using SmartDeskAI.Domain.Entities;

namespace SmartDeskAI.Application.Users.Commands.InviteUser
{
    public sealed class InviteUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentTenantService _currentTenant;

        public InviteUserCommandHandler(IUserRepository userRepository, ICurrentTenantService currentTenant)
        {
            _userRepository = userRepository;
            _currentTenant = currentTenant;
        }

        public async Task<Guid> Handle(InviteUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existing is not null)
                throw new InvalidOperationException($"Un utilisateur avec l'email '{request.Email}' existe déjà dans ce tenant.");

            var user = User.Invite(_currentTenant.TenantId, request.Email, request.Role);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangeAsync(cancellationToken);

            return user.Id;
        }
    }
}
