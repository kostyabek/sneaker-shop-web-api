using System.Collections.Generic;

namespace BaseCamp_WEB_API.Core.Entities.Authorization
{
    /// <summary>
    /// Represents a set of policies attached to a user.
    /// </summary>
    public class UserWithPolicies : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets a set of policies with claims.
        /// </summary>
        public IDictionary<string, IDictionary<string, string>> Policies { get; set; }
    }
}