using System;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Conversion;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Persistance.Dao;

namespace Dmarc.Common.Report.Email
{
    public interface IPersistentEmailMessageInfoProcessor<TDomain> : IEmailMessageInfoProcessor<TDomain> where TDomain : class
    { }

    public class PersistentEmailMessageInfoProcessor<TDomain, TEntity> : IPersistentEmailMessageInfoProcessor<TDomain> where TDomain : class
    {
        private readonly IEmailMessageInfoProcessor<TDomain> _processor;
        private readonly IToEntityConverter<TDomain, TEntity> _converter;
        private readonly IReportDaoAsync<TEntity> _dao;
        private readonly ILogger _log;

        public PersistentEmailMessageInfoProcessor(IEmailMessageInfoProcessor<TDomain> processor, 
            IToEntityConverter<TDomain, TEntity> converter,
            IReportDaoAsync<TEntity> dao,
            ILogger log)
        {
            _processor = processor;
            _converter = converter;
            _dao = dao;
            _log = log;
        }

        public async Task<Result<TDomain>> ProcessEmailMessage(EmailMessageInfo messageInfo)
        {
            Result<TDomain> result = await _processor.ProcessEmailMessage(messageInfo);

            try
            {
                if (result.Success)
                {
                    TEntity entity = _converter.Convert(result.Report);
                    
                    _log.Info($"Persisting email message for message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}");

                    bool added = await _dao.Add(entity);

                    if (added)
                    {
                        _log.Info($"Successfully persisted email message for message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}");
                    }
                    else
                    {
                        _log.Info($"Did not persist email message for message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId} as it was a duplicate.");
                    }

                    return new Result<TDomain>(result.Report, result.Success, !added);
                }

                return result;
            }
            catch(Exception e)
            {
                _log.Error($"Failed to persist email message, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId} with error {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return Result<TDomain>.FailedResult;
            }
        }
    }
}