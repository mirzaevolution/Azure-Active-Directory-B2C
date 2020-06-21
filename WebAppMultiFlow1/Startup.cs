using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using WebAppMultiFlow1.Models;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;

namespace WebAppMultiFlow1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            var b2cConfig = Configuration.GetSection("AzureAdB2C");
            AzureAdB2COptions azureAdB2COptions = new AzureAdB2COptions();
            Configuration.Bind("AzureAdB2C", azureAdB2COptions);
            services.Configure<AzureAdB2COptions>(b2cConfig);


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Constants.SignInSignUpPolicy;

            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/Auth/Login");
                    options.LogoutPath = new PathString("/Auth/Logout");
                    options.ReturnUrlParameter = "returnUrl";
                })
                .AddOpenIdConnect(Constants.SignInSignUpPolicy, GetOpenIdConnectOptions(azureAdB2COptions, Constants.SignInSignUpPolicy))
                .AddOpenIdConnect(Constants.EditProfilePolicy, GetOpenIdConnectOptions(azureAdB2COptions, Constants.EditProfilePolicy));
            services.AddDistributedMemoryCache();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private Action<OpenIdConnectOptions> GetOpenIdConnectOptions(AzureAdB2COptions azureAdB2COptions, string policyName) =>
            (options) =>
            {
                options.Authority = azureAdB2COptions.GetB2CAuthority(policyName);
                options.ClientId = azureAdB2COptions.ClientId;
                options.ClientSecret = azureAdB2COptions.ClientSecret;
                options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.CallbackPath = $"/signin/{policyName}";
                options.SignedOutCallbackPath = $"/signout/{policyName}";
                options.SaveTokens = true;
                options.UseTokenLifetime = true;
                foreach (string scope in azureAdB2COptions.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    options.Scope.Add(scope);
                }
                options.Events.OnAuthorizationCodeReceived += OnAuthorizationCodeReceived;
                options.Events.OnRemoteFailure += OnRemoteFailureHandler;
            };

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            string authority = context.Options.Authority;
            string clientId = context.Options.ClientId;
            string clientSecret = context.Options.ClientSecret;
            string redirectUri = context.TokenEndpointRequest.RedirectUri;

            string code = context.TokenEndpointRequest.Code;
            IEnumerable<string> scopes = context.Options.Scope.ToList();

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithB2CAuthority(authority)
                .WithRedirectUri(redirectUri)
                .Build();

            var result = await app.AcquireTokenByAuthorizationCode(scopes, code)
                .WithB2CAuthority(authority)
              .ExecuteAsync();
            context.HandleCodeRedemption(result.AccessToken, result.IdToken);
        }

        private Task OnRemoteFailureHandler(RemoteFailureContext context)
        {
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    }
}
