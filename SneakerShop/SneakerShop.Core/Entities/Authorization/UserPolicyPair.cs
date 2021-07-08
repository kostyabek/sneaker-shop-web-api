namespace BaseCamp_WEB_API.Core.Entities.Authorization
{
    /// <summary>
    /// Represents a record from the "users_policies" table.
    /// </summary>
    public class UserPolicyPair : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets policy ID.
        /// </summary>
        public int PolicyId { get; set; }
    }
}