using BaseCamp_WEB_API.Core.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BaseCamp_WEB_API.Data.Contexts
{
    /// <summary>
    /// Context for working with <see cref="UserRoleWithPrivileges"/> DB data.
    /// </summary>
    public sealed class PolicyAuthorizationContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyAuthorizationContext"/> class.
        /// </summary>
        /// <param name="options">Context options.</param>
        public PolicyAuthorizationContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

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