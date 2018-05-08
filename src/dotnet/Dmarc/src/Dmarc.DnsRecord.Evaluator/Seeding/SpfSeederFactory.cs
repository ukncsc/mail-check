using System;
using Amazon.SQS;
using Dmarc.Common.Data;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Evaluator.Seeding.Config;
using Dmarc.DnsRecord.Evaluator.Seeding.Dao;
using Dmarc.DnsRecord.Evaluator.Seeding.Mapping;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.DnsRecord.Evaluator.Seeding
{
    public class SpfSeederFactory
    {
        public static ISeeder Create(ISeedingConfig config)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<ISeedingConfig>(p => config)
                .AddTransient<ISeeder, Seeder<SpfRecord, SpfConfigsUpdated>>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(config.ConnectionString))
                .AddTransient<IDnsRecordDao<SpfRecord>, SpfRecordDao>()
                .AddTransient<IMapper<SpfRecord, SpfConfigsUpdated>, SpfConfigMapper>()
                .AddTransient<ISpfConfigParser, SpfConfigParser>()
                .AddTransient<IAmazonSQS, AmazonSQSClient>()
                .BuildServiceProvider();

            return serviceProvider.GetService<ISeeder>();
        }
    }
}