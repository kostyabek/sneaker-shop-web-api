using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repository
{
    /// <summary>
    /// Works with the "seasons" table.
    /// </summary>
    public class SeasonRepository : GenericRepository<Season, int>, ISeasonRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKta object for query creation.</param>
        public SeasonRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "seasons";
        }
    }
}