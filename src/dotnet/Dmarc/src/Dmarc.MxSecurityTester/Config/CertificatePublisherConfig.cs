using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.MxSecurityTester.Config
{
    public interface ICertificatePublisherConfig : IPublisherConfig
    {
    }

    public class CertificatePublisherConfig : ICertificatePublisherConfig
    {
        public CertificatePublisherConfig(IEnvironmentVariables environmentVariables)
        {
            PublisherConnectionString = environmentVariables.Get("SnsCertsTopicArn");
        }

        public string PublisherConnectionString { get; }
    }
}
