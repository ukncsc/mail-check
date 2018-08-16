using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.AggregateReport.Parser.Lambda.Publishers
{
    public abstract class MessagePublisher : IMessagePublisher
    {
        private readonly IPublisher _publisher;
        private readonly ILogger _log;
        private readonly string _publisherConnectionString;

        protected MessagePublisher(IPublisher publisher, ILogger log, string publisherConnectionString)
        {
            _publisher = publisher;
            _log = log;
            _publisherConnectionString = publisherConnectionString;
        }

        public async Task Publish(AggregateReportInfo aggregateReportInfo)
        {
            List<object> messages = Create(aggregateReportInfo);

            _log.Info(messages.Any()
                ? $"Publishing {messages.Count} {messages.First().GetType().Name} messages."
                : $"No messages to publish for publisher: {GetType()}");

            foreach (var message in messages)
            {
                await _publisher.Publish(message, _publisherConnectionString);
            }
        }

        public abstract List<object> Create(AggregateReportInfo aggregateReportInfo);
    }
}