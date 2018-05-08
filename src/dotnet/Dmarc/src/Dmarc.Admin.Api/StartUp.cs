using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Dmarc.Admin.Api.Dao.Domain;
using Dmarc.Admin.Api.Dao.Group;
using Dmarc.Admin.Api.Dao.GroupDomain;
using Dmarc.Admin.Api.Dao.GroupUser;
using Dmarc.Admin.Api.Dao.Search;
using Dmarc.Admin.Api.Dao.User;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using Dmarc.Common.Api.Identity.Dao;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Api.Identity.Middleware;
using Dmarc.Common.Api.Middleware;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Dmarc.Common.PublicSuffix;

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
            .AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            })
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
            .AddTransient<IValidator<GetEntitiesByRelatedIdRequest>, GetEntitiesByRelatedIdRequestValidator>()
            .AddTransient<IValidator<ChangeMembershipRequest>, ChangeMembershipRequestValidator>()
            .AddTransient<IValidator<DomainForCreation>, DomainForCreationValidator>()
            .AddTransient<IDomainValidator, DomainValidator>()
            .AddTransient<IValidator<GroupForCreation>, GroupForCreationValidator>()
            .AddTransient<IValidator<UserForCreation>, UserForCreationValidator>()
            .AddTransient<IValidator<EntitySearchRequest>, EntitySearchRequestValidator>()
            .AddTransient<IValidator<AllEntitiesSearchRequest>, AllEntitiesSearchRequestValidator>()
            .AddTransient<IIdentityDao, IdentityDao>()
            .AddSingleton<IOrganisationalDomainProvider, OrganisationDomainProvider>()
            .AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            })
            .AddAuthorization(options =>
            {
                options.AddPolicy(PolicyType.Admin, policy => policy.RequireAssertion(context => context.User.Claims.Any(_ => _.Type == ClaimTypes.Role && _.Value == RoleType.Admin)));
            })
            .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole((st, logLevel) => logLevel >= LogLevel.Debug);

            app.UseMiddleware<IdentityMiddleware>()
                .UseMiddleware<UnhandledExceptionLoggingMiddleware>()
                .UseCors("CorsPolicy")
                .UseMvc();
        }
    }
}
