using BaseCamp_WEB_API.Core.Entities.Authorization.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BaseCamp_WEB_API.Core.Entities.Authorization.Attributes
{
    /// <summary>
    /// Attribute for validating incoming access requests for needed claims.
    /// </summary>
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimRequirementAttribute"/> class.
        /// </summary>
        /// <param name="actionType">Type of a claim to check.</param>
        /// <param name="resourceType">Value of a claim being checked.</param>
        public ClaimRequirementAttribute(string resourceType, string actionType) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { resourceType, actionType };
        }
    }
}