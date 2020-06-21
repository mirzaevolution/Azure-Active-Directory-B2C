using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using WebApiMultiFlow1.Models;

namespace WebApiMultiFlow1
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
            AzureADB2COptions options = new AzureADB2COptions();
            Configuration.Bind("AzureAdB2C", options);
            services.AddAuthentication(Constants.SignInSignUpPolicy)
                .AddJwtBearer(Constants.SignInSignUpPolicy, GetJwtBearerOptions(options, Constants.SignInSignUpPolicy))
                .AddJwtBearer(Constants.EditProfilePolicy, GetJwtBearerOptions(options, Constants.EditProfilePolicy));
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        public Action<JwtBearerOptions> GetJwtBearerOptions(AzureADB2COptions options, string policy)
        {
            return (o) =>
            {
                o.Authority = string.Format(options.Authority, policy);
                o.Audience = options.ClientId;
                o.SaveToken = true;
            };
        }
    }
}
