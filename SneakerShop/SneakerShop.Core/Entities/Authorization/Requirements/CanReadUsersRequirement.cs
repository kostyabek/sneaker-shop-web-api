using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BaseCamp_WEB_API.Core.Entities.Authorization.Requirements
{
    /// <summary>
    /// Requirement that users need to meet to access info about users.
    /// </summary>
    public class CanReadUsersRequirement : IAuthorizationRequirement
    {
    }

    public class CanReadUsersRequirementHandler : AuthorizationHandler<CanReadUsersRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadUsersRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "CanReadUsers"))
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}