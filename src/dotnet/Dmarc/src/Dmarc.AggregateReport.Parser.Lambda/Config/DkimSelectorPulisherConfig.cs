using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.AggregateReport.Parser.Lambda.Config
{
    public interface IDkimSelectorPublisherConfig : IPublisherConfig{}

    public class DkimSelectorPublisherConfig : IDkimSelectorPublisherConfig
    {
        public DkimSelectorPublisherConfig(IEnvironmentVariables environmentVariables)
        {
            PublisherConnectionString = environmentVariables.Get("DkimSelectorsTopicArn");
        }

        public string PublisherConnectionString { get; }
    }
}