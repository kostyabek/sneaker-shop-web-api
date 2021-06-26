using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by UserRepository.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User, int>
    {
        /// <summary>
        /// Fetches a user by his username.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Fetches a user by his email.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUserByEmailAsync(string email);
    }
}