using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityEvaluator.Dao;
using Dmarc.MxSecurityEvaluator.Domain;

namespace Dmarc.MxSecurityEvaluator.Processors
{
    public class TlsRecordProcessorManual : TlsRecordProcessor
    {
        private readonly List<int> _domainIds;

        public TlsRecordProcessorManual(
            IMxSecurityEvaluatorArguments arguments,
            ITlsRecordDao tlsRecordDao,
            IMxSecurityEvaluator mxSecurityEvaluator,
            ILogger log) : base(tlsRecordDao, mxSecurityEvaluator, log)
        {
            _domainIds = arguments.DomainIds;
        }

        public override async Task Run()
        {
            await Task.WhenAll(_domainIds.Select(ProcessTlsConnectionResults));
        }
    }
}
