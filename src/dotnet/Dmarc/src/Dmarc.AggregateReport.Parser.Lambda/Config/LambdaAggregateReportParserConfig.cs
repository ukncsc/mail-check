using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Report.Config;

namespace Dmarc.AggregateReport.Parser.Lambda.Config
{
    public class LambdaAggregateReportParserConfig : LambdaReportParserConfig, IPublisherConfig
    {
        public LambdaAggregateReportParserConfig(IEnvironmentVariables environmentVariables) 
            : base(environmentVariables)
        {
            PublisherConnectionString = environmentVariables.Get("SnsTopicArn");
        }

        public string PublisherConnectionString { get; }
    }
}
