using SmartDeskAI.Domain.Entities;

namespace SmartDeskAI.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Contrat de dépôt (Repository) pour la gestion de la persistance des utilisateurs.
        /// Il définit les opérations d'accès aux données nécessaires à la couche Application
        /// sans dépendre d'une technologie spécifique (ex: EF Core, Dapper).
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task SaveChangeAsync(CancellationToken cancellationToken);
    }
}
