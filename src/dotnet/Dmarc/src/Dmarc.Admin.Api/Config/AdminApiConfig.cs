using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.Admin.Api.Config
{
    public class AdminApiConfig : IPublisherConfig
    {
        public AdminApiConfig(IEnvironmentVariables environmentVariables)
        {
            PublisherConnectionString = environmentVariables.Get("SnsTopicArn");
        }

        public string PublisherConnectionString { get; }
    }
}
