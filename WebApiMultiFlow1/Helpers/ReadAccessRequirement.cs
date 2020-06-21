using Microsoft.AspNetCore.Authorization;
namespace WebApiMultiFlow1.Helpers
{
    public class ReadAccessRequirement : IAuthorizationRequirement
    {
        public string ClaimType => "scp";
        public string ClaimValue => Constants.UserReadScope;
    }
}
