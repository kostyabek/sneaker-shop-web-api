using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BaseCamp_WEB_API.Core.Entities.Authorization.Filters
{
    /// <summary>
    /// Checks incoming access requests for needed claims.
    /// </summary>
    public class ClaimRequirementFilter : IAsyncAuthorizationFilter
    {
        private readonly string _resourceType;
        private readonly string _actionType;
        private int _userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimRequirementFilter"/> class.
        /// </summary>
        /// <param name="resourceType">Resource type to check.</param>
        /// /// <param name="actionType">Action type to check.</param>
        public ClaimRequirementFilter(string resourceType, string actionType)
        {
            _resourceType = resourceType;
            _actionType = actionType;
        }

        /// <inheritdoc/>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var repository = context.HttpContext.RequestServices.GetService<IUserRoleWithPrivilegesRepository>();
            int.TryParse(context.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value, out _userId);
            var queryResult = await repository.GetPrivilegesForResourceByUserId(_userId, _resourceType);
            var userRoleWithPrivileges = queryResult.SingleOrDefault();

            var allowed = GetAllowedOperations(userRoleWithPrivileges);
            foreach (var actionType in allowed)
            {
                if (string.Equals(_actionType, actionType, StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }
            }

            context.Result = new ForbidResult();
        }

        private static IEnumerable<string> GetAllowedOperations(UserRoleWithPrivileges userRoleWithPrivileges)
        {
            var allowed = new List<string>();
            if (userRoleWithPrivileges != null)
            {
                var privileges = userRoleWithPrivileges
                    .GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType.Name == "Boolean");
                foreach (var privilege in privileges)
                {
                    if ((bool)privilege.GetValue(userRoleWithPrivileges))
                    {
                        allowed.Add(privilege.Name);
                    }
                }
            }

            return allowed;
        }
    }
}