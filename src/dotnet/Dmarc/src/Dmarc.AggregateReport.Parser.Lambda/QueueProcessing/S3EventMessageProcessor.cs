using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.SQS.Model;
using Dmarc.AggregateReport.Parser.Lambda.Email;
using Dmarc.Common.Logging;

namespace Dmarc.AggregateReport.Parser.Lambda.QueueProcessing
{
    internal interface IMessageProcessor
    {
        Task<bool> TryProcessMessage(ILambdaContext context, Message message);
    }

    internal class S3EventMessageProcessor : IMessageProcessor
    {
        private readonly IS3EmailMessageProcessor _s3EmailMessageProcessor;
        private readonly IS3EventDeserializer _s3EventDeserializer;
        private readonly ILogger _log;

        public S3EventMessageProcessor(IS3EmailMessageProcessor s3EmailMessageProcessor,
            IS3EventDeserializer s3EventDeserializer,
            ILogger log)
        {
            _s3EmailMessageProcessor = s3EmailMessageProcessor;
            _s3EventDeserializer = s3EventDeserializer;
            _log = log;
        }

        public async Task<bool> TryProcessMessage(ILambdaContext context, Message message)
        {
            S3Event s3Event;
            if (_s3EventDeserializer.TryDeserializeS3Event(message.Body, out s3Event))
            {
                if (s3Event.Records == null || s3Event.Records.Count == 0)
                {
                    _log.Warn("S3 event contained no records.");
                    return false;
                }
                
                await _s3EmailMessageProcessor.ProcessEmailMessage(context, s3Event);
                return true;
            }

            _log.Error($"Skipping processing message as wasnt valid S3Event. Message had following body: {Environment.NewLine} {message.Body}");
            return false;
        }   
    }
}