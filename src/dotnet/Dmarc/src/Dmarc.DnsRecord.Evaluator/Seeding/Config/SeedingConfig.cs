using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.DnsRecord.Evaluator.Seeding.Config
{
    public interface ISeedingConfig
    {
        string ConnectionString { get; }
        string SqsQueueUrl { get; }
    }

    public class SeedingConfig : ISeedingConfig
    {
        public SeedingConfig(string connectionString, string sqsQueueUrl)
        {
            ConnectionString = connectionString;
            SqsQueueUrl = sqsQueueUrl;
        }

        public string ConnectionString { get; }
        public string SqsQueueUrl { get; }
    }
}
