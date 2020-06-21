namespace WebApiMultiFlow1.Models
{
    public class AzureADB2COptions
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public string ClientId { get; set; }
        public string SignInSignUpPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
    }
}
