using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.SQS.Model;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Email;

namespace Dmarc.Common.Report.QueueProcessing
{
    public interface IMessageProcessor
    {
        Task<bool> TryProcessMessage(ILambdaContext context, Message message);
    }

    public class S3EventMessageProcessor : IMessageProcessor
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
                _log.Info($"Successfully deserialised S3Event for message Id: {message.MessageId} for request Id: {context.AwsRequestId}");
                return await _s3EmailMessageProcessor.ProcessEmailMessage(message.MessageId, context, s3Event);
            }

            _log.Error($"Skipping processing message as wasnt valid S3Event. Message had following body: {System.Environment.NewLine} {message.Body}");
            return false;
        }   
    }
}