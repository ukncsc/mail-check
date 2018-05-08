using System;
using Amazon.SQS;
using Dmarc.Common.Data;
using Dmarc.DnsRecord.Contract.Messages;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using Dmarc.DnsRecord.Evaluator.Seeding.Config;
using Dmarc.DnsRecord.Evaluator.Seeding.Dao;
using Dmarc.DnsRecord.Evaluator.Seeding.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.DnsRecord.Evaluator.Seeding
{
    public class DmarcSeederFactory
    {
        public static ISeeder Create(ISeedingConfig config)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
               .AddTransient<ISeedingConfig>(p => config)
               .AddTransient<ISeeder, Seeder<DmarcRecord, DmarcConfigsUpdated>> ()
               .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(config.ConnectionString))
               .AddTransient<IDnsRecordDao<DmarcRecord>, DmarcRecordDao>()
               .AddTransient<IMapper<DmarcRecord, DmarcConfigsUpdated>, DmarcConfigMapper>()
               .AddTransient<IDmarcConfigParser, DmarcConfigParser>()
               .AddTransient<IAmazonSQS, AmazonSQSClient>()
               .BuildServiceProvider();

            return serviceProvider.GetService<ISeeder>();
        }
    }
}
