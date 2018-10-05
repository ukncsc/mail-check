using Amazon.SimpleSystemsManagement;
using Dmarc.Common.Api.Handlers;
using Dmarc.Common.Api.Identity.Authentication;
using Dmarc.Common.Api.Identity.Dao;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Api.Middleware;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.PublicSuffix;
using Dmarc.Common.Validation;
using Dmarc.DomainStatus.Api.Config;
using Dmarc.DomainStatus.Api.Dao.DomainStatus;
using Dmarc.DomainStatus.Api.Dao.DomainStatusList;
using Dmarc.DomainStatus.Api.Dao.Permission;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Services;
using Dmarc.DomainStatus.Api.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dmarc.DomainStatus.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddHealthChecks(HealthCheckOptions)
                .AddCors(CorsOptions)
                .AddAuthorization(AuthorizationOptions)
                .AddAuthentication(AuthenticationSchemeName)
                .AddMailCheckAuthentication();

            services
                .AddTransient<IPersistanceConnectionTester, DatabaseConnectionTester>()
                .AddTransient<IValidator<DomainRequest>, DomainRequestValidator>()
                .AddTransient<IValidator<DomainsRequest>, DomainsRequestValidator>()
                .AddTransient<IValidator<DateRangeDomainRequest>, DateRangeDomainRequestValidator>()
                .AddSingleton<IDomainStatusApiConfig, DomainStatusApiConfig>()
                .AddTransient<IDomainStatusDao, DomainStatusDao>()
                .AddTransient<IDomainStatusListDao, DomainStatusListDao>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IAmazonSimpleSystemsManagement>(p => new AmazonSimpleSystemsManagementClient())
                .AddTransient<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IDomainValidator, DomainValidator>()
                .AddTransient<IIdentityDao, IdentityDao>()
                .AddSingleton<IOrganisationalDomainProvider, OrganisationDomainProvider>()
                .AddTransient<IPermissionDao, PermissionDao>()
                .AddTransient<IReverseDnsApi, ReverseDnsApiClient>()
                .AddTransient<ICertificateEvaluatorApi, CertificateEvaluatorApiClient>()
                .AddSingleton<IDomainValidator, DomainValidator>()
                .AddSingleton<IPublicDomainListValidator, PublicDomainListValidator>();

            services
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole((st, logLevel) => logLevel >= LogLevel.Debug);

            app
                .UseAuthentication()
                .UseMiddleware<UnhandledExceptionMiddleware>()
                .UseCors(CorsPolicyName)
                .UseMvc();
        }

        private static Action<AuthorizationOptions> AuthorizationOptions => options =>
        {
            options.AddPolicy(PolicyType.Admin, policy => policy.RequireAssertion(context => context.User.Claims.Any(_ => _.Type == ClaimTypes.Role && _.Value == RoleType.Admin)));
        };

        private static Action<CorsOptions> CorsOptions => options =>
        {
            options.AddPolicy(CorsPolicyName, builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        };

        private static Action<HealthCheckBuilder> HealthCheckOptions => checks =>
        {
            checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
        };

        private const string AuthenticationSchemeName = "Automatic";

        private const string CorsPolicyName = "CorsPolicy";
    }
}
