using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities.Authentication;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repositories
{
    /// <summary>
    /// Works with the "refresh_tokens" table.
    /// </summary>
    public class RefreshTokenRepository : GenericRepository<RefreshToken, int>, IRefreshTokenRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKata object for query creation.</param>
        public RefreshTokenRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "refresh_tokens";
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<RefreshToken>> GetByRefreshTokenValueAsync(string refreshToken)
        {
            return queryFactory
                .Query(NodeTableName)
                .Where("token", refreshToken)
                .GetAsync<RefreshToken>();
        }
    }
}