using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repositories
{
    /// <summary>
    /// Works with the "sneakers" table.
    /// </summary>
    public class SneakerRepository : GenericRepository<Sneaker, int>, ISneakerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SneakerRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKata object for query creation.</param>
        public SneakerRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "sneakers";
        }

        /// <summary>
        /// Puts correspondent field of a Sneaker object to uppercase.
        /// </summary>
        /// <param name="sneaker">Contains data to process.</param>
        public static void PutFieldsToUppercase(Sneaker sneaker)
        {
            sneaker.ModelName = sneaker.ModelName.ToUpper();
        }
    }
}