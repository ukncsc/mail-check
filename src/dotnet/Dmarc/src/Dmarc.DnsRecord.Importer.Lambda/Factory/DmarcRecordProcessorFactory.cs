using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Mapping;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Messaging.Sns.Publisher;
using Dmarc.Common.PublicSuffix;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Dmarc;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dns;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client;
using Dmarc.DnsRecord.Importer.Lambda.Mapping;
using Dmarc.DnsRecord.Importer.Lambda.RecordProcessor;
using Heijden.Dns.Portable;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.DnsRecord.Importer.Lambda.Factory
{
    public static class DmarcRecordProcessorFactory
    {
        public static IDnsRecordProcessor Create(ILogger log)
        {
            return new ServiceCollection()
                .AddSingleton(log)
                .AddTransient<IDnsNameServerProvider, LinuxDnsNameServerProvider>()
                .AddTransient(CreateDnsResolverWrapper)
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IRecordImporterConfig, RecordImporterConfig>()
                .AddTransient<IPublisherConfig, RecordImporterConfig>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IAmazonSimpleSystemsManagement>(p => new AmazonSimpleSystemsManagementClient(new EnvironmentVariablesAWSCredentials()))
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddSingleton<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IDnsRecordDao, DmarcRecordDao>()
                .AddTransient<IDnsRecordClient, DmarcRecordDnsClient>()
                .AddTransient<IOrganisationalDomainProvider, OrganisationDomainProvider>()
                .AddTransient<IPublisher, SnsPublisher>()
                .AddTransient<IMapper<List<RecordEntity>, DnsRecordMessage>, DmarcConfigsUpdatedMapper>()
                .AddTransient<IAmazonSimpleNotificationService>(provider => new AmazonSimpleNotificationServiceClient(new EnvironmentVariablesAWSCredentials()))
                .AddDnsRecordProcessor()
                .BuildServiceProvider()
                .GetRequiredService<IDnsRecordProcessor>();
        }

        private static IDnsResolver CreateDnsResolverWrapper(IServiceProvider provider)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new DnsResolverWrapper(Resolver.DefaultDnsServers.ToList())
                : new DnsResolverWrapper(provider.GetRequiredService<IDnsNameServerProvider>().GetNameServers().Select(_ => new IPEndPoint(_, 53)).ToList());
        }

        private static IServiceCollection AddDnsRecordProcessor(this IServiceCollection services)
        {
            services.AddTransient<IDnsRecordUpdater, DnsRecordUpdater>();

            services.AddTransient<PersistantDnsRecordUpdater>(_ => new PersistantDnsRecordUpdater(
                _.GetRequiredService<IDnsRecordUpdater>(),
                _.GetRequiredService<IDnsRecordDao>()));

            services.AddTransient<PublishingDnsRecordUpdater>(_ => new PublishingDnsRecordUpdater(
                _.GetRequiredService<PersistantDnsRecordUpdater>(),
                _.GetRequiredService<IMapper<List<RecordEntity>, DnsRecordMessage>>(),
                _.GetRequiredService<IPublisher>(),
                _.GetRequiredService<IPublisherConfig>()));

            services.AddTransient<IDnsRecordProcessor>(_ => new DnsRecordProcessor(
                _.GetRequiredService<IDnsRecordDao>(),
                _.GetRequiredService<PublishingDnsRecordUpdater>(),
                _.GetRequiredService<IRecordImporterConfig>(),
                _.GetRequiredService<ILogger>()));

            return services;
        }
    }
}