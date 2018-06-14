using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Dmarc.Common.Interface.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dmarc.Common.Messaging.Sns.Publisher
{
    public class SnsPublisher : IPublisher
    {
        private const string Type = "Type";
        private const string Version = "Version";

        private readonly IAmazonSimpleNotificationService _simpleNotificationService;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public SnsPublisher(IAmazonSimpleNotificationService simpleNotificationService)
        {
            _simpleNotificationService = simpleNotificationService;
        }

        public async Task Publish<T>(T message, string topic)
        {
            string stringMessage = JsonConvert.SerializeObject(message, _serializerSettings);

            PublishRequest publishRequest = new PublishRequest(topic, stringMessage)
            {
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { Type, new MessageAttributeValue
                        {
                            StringValue = message.GetType().Name,
                            DataType = "String"
                        }
                    },
                    { Version, new MessageAttributeValue
                        {
                            StringValue = GetVersion(message.GetType().GetTypeInfo().Assembly.GetName().Version),
                            DataType = "String"
                        }
                    }
                }
            };

            await _simpleNotificationService.PublishAsync(publishRequest);
        }

        private string GetVersion(Version version)
        {
            return $"{version.Major}.{version.Minor}.{version.Revision}";
        }
    }
}