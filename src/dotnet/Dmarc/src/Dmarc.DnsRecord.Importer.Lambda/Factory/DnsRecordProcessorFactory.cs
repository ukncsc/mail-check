using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Mapping;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Messaging.Sns.Publisher;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Importer.Lambda.Config;
using Dmarc.DnsRecord.Importer.Lambda.Dao;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Dmarc;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Entities;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Mx;
using Dmarc.DnsRecord.Importer.Lambda.Dao.Spf;
using Dmarc.DnsRecord.Importer.Lambda.Dns;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client;
using Dmarc.DnsRecord.Importer.Lambda.Mapping;
using Dmarc.DnsRecord.Importer.Lambda.Publisher;
using Dmarc.DnsRecord.Importer.Lambda.RecordProcessor;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.DnsRecord.Importer.Lambda.Factory
{
    public static class DnsRecordProcessorFactory
    {
        public static IDnsRecordProcessor CreateMxProcessor(ILogger log)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddCommonDependendencies(log)
                .AddTransient<IDnsRecordUpdater, DnsRecordUpdater>()
                .AddTransient<IDnsRecordDao, MxRecordDao>()
                .AddTransient<IDnsRecordClient, MxRecordDnsClient>()
                .BuildServiceProvider();

            return serviceProvider.GetService<IDnsRecordProcessor>();
        }

        public static IDnsRecordProcessor CreateSpfProcessor(ILogger log)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddCommonDependendencies(log)
                .AddTransient<IDnsRecordUpdater, PublishingDnsRecordUpdater>()
                .AddTransient<IDnsRecordDao, SpfRecordDao>()
                .AddTransient<IDnsRecordClient, SpfRecordDnsClient>()
                .AddTransient<IRecordEntityPublisher, RecordEntityPublisher>()
                .AddTransient<IPublisher, SnsPublisher>()
                .AddTransient<IMapper<List<RecordEntity>, DnsRecordMessage>, SpfConfigsUpdatedMapper>()
                .AddTransient<IAmazonSimpleNotificationService>(provider => new AmazonSimpleNotificationServiceClient(new EnvironmentVariablesAWSCredentials()))
                .BuildServiceProvider();

            return serviceProvider.GetService<IDnsRecordProcessor>();
        }

        public static IDnsRecordProcessor CreateDmarcProcessor(ILogger log)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddCommonDependendencies(log)
                .AddTransient<IDnsRecordUpdater, PublishingDnsRecordUpdater>()
                .AddTransient<IDnsRecordDao, DmarcRecordDao>()
                .AddTransient<IDnsRecordClient, DmarcRecordDnsClient>()
                .AddTransient<IRecordEntityPublisher, RecordEntityPublisher>()
                .AddTransient<IPublisher, SnsPublisher>()
                .AddTransient<IMapper<List<RecordEntity>, DnsRecordMessage>, DmarcConfigsUpdatedMapper>()
                .AddTransient<IAmazonSimpleNotificationService>(provider => new AmazonSimpleNotificationServiceClient(new EnvironmentVariablesAWSCredentials()))
                .BuildServiceProvider();

            return serviceProvider.GetService<IDnsRecordProcessor>();
        }

        private static IServiceCollection AddCommonDependendencies(this IServiceCollection serviceCollection, ILogger log)
        {
            return serviceCollection.AddSingleton(log)
                .AddTransient<IDnsNameServerProvider, LinuxDnsNameServerProvider>()
                .AddTransient<IDnsResolver, DnsResolverWrapper>()
                .AddTransient<IDnsRecordProcessor, DnsRecordProcessor>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IRecordImporterConfig, RecordImporterConfig>()
                .AddTransient<IPublisherConfig, RecordImporterConfig>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IAmazonSimpleSystemsManagement>(p => new AmazonSimpleSystemsManagementClient(new EnvironmentVariablesAWSCredentials()))
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddSingleton<IConnectionInfoAsync, ConnectionInfoAsync>();
        }
    }
}
