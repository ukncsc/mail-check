using System;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Parser;

namespace Dmarc.Common.Report.Email
{
    public interface IEmailMessageInfoProcessor<TDomain> where TDomain : class
    {
        Task<Result<TDomain>> ProcessEmailMessage(EmailMessageInfo messageInfo);
    }

    public class EmailMessageInfoProcessor<TDomain> : IEmailMessageInfoProcessor<TDomain>
        where TDomain : class
    {
        private readonly IReportParser<TDomain> _parser;
        private readonly ILogger _log;

        public EmailMessageInfoProcessor(IReportParser<TDomain> parser,
            ILogger log)
        {
            _parser = parser;
            _log = log;
        }

        public Task<Result<TDomain>> ProcessEmailMessage(EmailMessageInfo messageInfo)
        {
            try
            {
                _log.Info($"Parsing email message, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}.");

                TDomain report = _parser.Parse(messageInfo);

                _log.Info($"Successfully parsed email message, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}.");

                return Task.FromResult(new Result<TDomain>(report, true, false));
            }
            catch (Exception e)
            {
                _log.Error($"Failed to process email message, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId} with error {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return Task.FromResult(Result<TDomain>.FailedResult);
            }
        }
    }
}