using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repository
{
    /// <summary>
    /// Works with the 'vendors' table.
    /// </summary>
    public class VendorRepository : GenericRepository<Vendor, int>, IVendorRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VendorRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKta object for query creation.</param>
        public VendorRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "vendors";
        }
    }
}