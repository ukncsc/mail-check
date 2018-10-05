using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Dmarc.Admin.Api.Config;
using Dmarc.Admin.Api.Dao.Domain;
using Dmarc.Admin.Api.Dao.Group;
using Dmarc.Admin.Api.Dao.GroupDomain;
using Dmarc.Admin.Api.Dao.GroupUser;
using Dmarc.Admin.Api.Dao.Search;
using Dmarc.Admin.Api.Dao.User;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using Dmarc.Common.Api.Identity.Authentication;
using Dmarc.Common.Api.Identity.Dao;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Api.Middleware;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Messaging.Sns.Publisher;
using Dmarc.Common.PublicSuffix;
using Dmarc.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Dmarc.Admin.Api
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
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IAmazonSimpleSystemsManagement>(p => new AmazonSimpleSystemsManagementClient())
                .AddSingleton<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IUserDao, UserDao>()
                .AddTransient<IGroupDao, GroupDao>()
                .AddTransient<IDomainDao, DomainDao>()
                .AddTransient<IGroupUserDao, GroupUserDao>()
                .AddTransient<IGroupDomainDao, GroupDomainDao>()
                .AddTransient<ISearchDao, SearchDao>()
                .AddTransient<IPublicDomainListValidator, PublicDomainListValidator>()
                .AddTransient<IValidator<GetEntitiesByRelatedIdRequest>, GetEntitiesByRelatedIdRequestValidator>()
                .AddTransient<IValidator<ChangeMembershipRequest>, ChangeMembershipRequestValidator>()
                .AddTransient<IValidator<DomainForCreation>, DomainForCreationValidator>()
                .AddTransient<IDomainValidator, DomainValidator>()
                .AddTransient<IValidator<GroupForCreation>, GroupForCreationValidator>()
                .AddTransient<IValidator<UserForCreation>, UserForCreationValidator>()
                .AddTransient<IValidator<EntitySearchRequest>, EntitySearchRequestValidator>()
                .AddTransient<IValidator<AllEntitiesSearchRequest>, AllEntitiesSearchRequestValidator>()
                .AddTransient<IValidator<PublicDomainForCreation>, PublicDomainValidator>()
                .AddTransient<IIdentityDao, IdentityDao>()
                .AddTransient<IPublisher, SnsPublisher>()
                .AddTransient<IPublisherConfig, AdminApiConfig>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>();

            services
                .AddMvc();
        }

        private static Action<AuthorizationOptions> AuthorizationOptions => options =>
        {
            options.AddPolicy(PolicyType.Standard, policy =>
            {
                policy.RequireAssertion(context => context.User.Claims.Any(_ => _.Type == ClaimTypes.Role && _.Value == RoleType.Standard || _.Value == RoleType.Admin));
            });

            options.AddPolicy(PolicyType.Admin, policy =>
            {
                policy.RequireAssertion(context => context.User.Claims.Any(_ => _.Type == ClaimTypes.Role && _.Value == RoleType.Admin));
            });
        };
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole((st, logLevel) => logLevel >= LogLevel.Debug);

            app
                .UseAuthentication()
                .UseMiddleware<UnhandledExceptionMiddleware>()
                .UseCors(CorsPolicyName)
                .UseMvc();
        }

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
