namespace BaseCamp_WEB_API.Core.Entities.Authorization
{
    /// <summary>
    /// Represents a database record of a claim.
    /// </summary>
    public class ClaimRecord : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets claim type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets claim value.
        /// </summary>
        public string Value { get; set; }
    }
}