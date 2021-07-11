using System.Threading;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BaseCamp_WEB_API.Core.DbContextAbstractions
{
    /// <summary>
    /// Serves as an abstraction layer for simplified mocking during unit testing.
    /// </summary>
    public interface IPolicyAuthorizationDbContext
    {
        /// <summary>
        /// Gets or sets collection of <see cref="ClaimRecord"/> records.
        /// </summary>
        public DbSet<ClaimRecord> ClaimRecords { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="PolicyRecord"/> records.
        /// </summary>
        public DbSet<PolicyRecord> PolicyRecords { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="PolicyClaimPair"/> records.
        /// </summary>
        public DbSet<PolicyClaimPair> PolicyClaimPairRecords { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="PolicyRecord"/> records.
        /// </summary>
        public DbSet<UserPolicyPair> UserPolicyPairRecords { get; set; }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}