using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Dmarc.AggregateReport.Api.Dao.Aggregated;
using Dmarc.AggregateReport.Api.Dao.Daily;
using Dmarc.AggregateReport.Api.Dao.Domain;
using Dmarc.AggregateReport.Api.Dao.Sender;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.AggregateReport.Api.Validation;
using Dmarc.Common.Api.Handlers;
using Dmarc.Common.Api.Identity.Dao;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Data;
using Dmarc.Common.Api.Middleware;
using Dmarc.Common.Encryption;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Dmarc.Common.Api.Identity.Authentication;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace Dmarc.AggregateReport.Api
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
                .AddTransient<ISenderStatisticsDao, SenderStatisticsDao>()
                .AddTransient<IAggregatedStatisticsDao, AggregatedStatisticsDao>()
                .AddTransient<IDailyStatisticsDao, DailyStatisticDao>()
                .AddTransient<IDomainsDao, DomainsDao>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>()
                .AddTransient<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IValidator<DomainSearchRequest>, DomainSearchRequestValidator>()
                .AddTransient<IValidator<DateRangeDomainRequest>, DateRangeDomainRequestValidator>()
                .AddTransient<IPersistanceConnectionTester, DatabaseConnectionTester>()
                .AddTransient<IIdentityDao, IdentityDao>();

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
            options.AddPolicy(PolicyType.Admin, policy =>
            {
                policy.RequireAssertion(context => context.User.Claims.Any(_ => _.Type == ClaimTypes.Role && _.Value == RoleType.Admin));
            });
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
