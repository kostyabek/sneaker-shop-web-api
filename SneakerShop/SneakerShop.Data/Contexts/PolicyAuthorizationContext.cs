using BaseCamp_WEB_API.Core.DbContextAbstractions;
using BaseCamp_WEB_API.Core.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BaseCamp_WEB_API.Data.Contexts
{
    /// <summary>
    /// Context for working with <see cref="UserRoleWithPrivileges"/> DB data.
    /// </summary>
    public sealed class PolicyAuthorizationContext : DbContext, IPolicyAuthorizationDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyAuthorizationContext"/> class.
        /// </summary>
        /// <param name="options">Context options.</param>
        public PolicyAuthorizationContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        /// <inheritdoc/>
        public DbSet<ClaimRecord> ClaimRecords { get; set; }

        /// <inheritdoc/>
        public DbSet<PolicyRecord> PolicyRecords { get; set; }

        /// <inheritdoc/>
        public DbSet<PolicyClaimPair> PolicyClaimPairRecords { get; set; }

        /// <inheritdoc/>
        public DbSet<UserPolicyPair> UserPolicyPairRecords { get; set; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClaimRecord>()
                .ToTable("claims");
            modelBuilder.Entity<PolicyRecord>()
                .ToTable("policies");
            modelBuilder.Entity<PolicyClaimPair>()
                .ToTable("policies_claims");
            modelBuilder.Entity<UserPolicyPair>()
                .ToTable("users_policies");
        }
    }
}