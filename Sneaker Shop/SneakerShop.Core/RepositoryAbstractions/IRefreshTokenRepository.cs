using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities.Authentication;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by RefreshTokenRepository.
    /// </summary>
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken, int>
    {
        /// <summary>
        /// Fetches a refresh token object by a provided token value.
        /// </summary>
        /// <param name="refreshToken">Refresh token object.</param>
        /// <returns></returns>
        Task<IEnumerable<RefreshToken>> GetByRefreshTokenValueAsync(string refreshToken);
    }
}