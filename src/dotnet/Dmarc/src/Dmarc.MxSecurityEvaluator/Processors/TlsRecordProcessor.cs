using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Dao;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Util;

namespace Dmarc.MxSecurityEvaluator.Processors
{
    public interface ITlsRecordProcessor
    {
        Task Run();
    }

    public abstract class TlsRecordProcessor : ITlsRecordProcessor
    {
        private readonly ILogger _log;
        private readonly IMxSecurityEvaluator _mxSecurityEvaluator;
        private readonly ITlsRecordDao _tlsRecordDao;

        protected TlsRecordProcessor(
            ITlsRecordDao tlsRecordDao,
            IMxSecurityEvaluator mxSecurityEvaluator,
            ILogger log)
        {
            _tlsRecordDao = tlsRecordDao;
            _mxSecurityEvaluator = mxSecurityEvaluator;
            _log = log;
        }

        public abstract Task Run();

        protected async Task ProcessTlsConnectionResults(int domainId)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var tlsConnectionResults = await _tlsRecordDao.GetDomainTlsConnectionResults(domainId);

            await Task.WhenAll(tlsConnectionResults.Select(EvaluateMxRecordProfile));

            stopwatch.Stop();

            _log.Debug($"Processed domain with ID {domainId}. Took {stopwatch.Elapsed.TotalSeconds} seconds.");
        }

        protected async Task EvaluateMxRecordProfile(MxRecordTlsProfile mxRecordTlsProfile)
        {
            if (mxRecordTlsProfile.MxHostname == null)
            {
                await _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile, EvaluatorResults.EmptyResults);

                _log.Debug(
                    $"MX record with ID {mxRecordTlsProfile.MxRecordId} has no hostname, saving null results.");
            }
            else if (mxRecordTlsProfile.ConnectionResults.HasFailedConnection())
            {
                await _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile, EvaluatorResults.ConnectionFailedResults);

                _log.Debug(
                    $"MX record with ID {mxRecordTlsProfile.MxRecordId} TLS connection failed, saving single inconclusive result.");
            }
            else
            {
                var tlsEvaluatorResults = _mxSecurityEvaluator.Evaluate(mxRecordTlsProfile.ConnectionResults);

                await _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile, tlsEvaluatorResults);

                _log.Debug(
                    $"Evaluated TLS connection results for MX record with ID {mxRecordTlsProfile.MxRecordId}.");
            }
        }

        
    }
}
