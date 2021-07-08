namespace BaseCamp_WEB_API.Core.Entities.Authorization
{
    /// <summary>
    /// Represents a record from the "policies_claims" table.
    /// </summary>
    public class PolicyClaimPair : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets policy ID,
        /// </summary>
        public int PolicyId { get; set; }

        /// <summary>
        /// Gets or sets claim ID for this policy.
        /// </summary>
        public int ClaimId { get; set; }
    }
}