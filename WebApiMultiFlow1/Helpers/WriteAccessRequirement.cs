using Microsoft.AspNetCore.Authorization;
namespace WebApiMultiFlow1.Helpers
{
    public class WriteAccessRequirement : IAuthorizationRequirement
    {
        public string ClaimType => "scp";
        public string ClaimValue => Constants.UserWriteScope;
    }
}
