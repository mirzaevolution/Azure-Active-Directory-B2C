using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace WebApiMultiFlow1.Helpers
{
    public class WriteAccessScopeHandler : AuthorizationHandler<WriteAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WriteAccessRequirement requirement)
        {
            string scope = context.User.FindFirstValue(requirement.ClaimType);
            if (!string.IsNullOrEmpty(scope))
            {
                string[] scopeArray = scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (scopeArray.Any(c => c.Equals(requirement.ClaimValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
