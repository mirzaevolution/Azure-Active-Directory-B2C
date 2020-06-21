namespace WebAppMultiFlow1.Models
{
    public class AzureAdB2COptions
    {
        public string Instance { get; set; }
        public string Tenant { get; set; }
        public string Scopes { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SignInSignUpPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string GetB2CAuthority(string policy)
        {
            return $"{Instance}/tfp/{Tenant}/{policy}/v2.0";
        }
    }
}
