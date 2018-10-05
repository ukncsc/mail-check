using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityEvaluator.Dao;
using Dmarc.MxSecurityEvaluator.Domain;

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
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<MxRecordTlsProfile> tlsConnectionResults = await _tlsRecordDao.GetDomainTlsConnectionResults(domainId);

            await Task.WhenAll(tlsConnectionResults.Select(EvaluateMxRecordProfile));

            stopwatch.Stop();

            _log.Debug($"Processed domain with ID {domainId}. Took {stopwatch.Elapsed.TotalSeconds} seconds.");
        }

        protected Task EvaluateMxRecordProfile(MxRecordTlsProfile mxRecordTlsProfile)
        {
            if (mxRecordTlsProfile.MxHostname == null)
            {
                _log.Debug($"No hostname for MX record with ID {mxRecordTlsProfile.MxRecordId}.");

                return _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile, EvaluatorResults.EmptyResults);
            }

            if (mxRecordTlsProfile.ConnectionResults.HasFailedConnection())
            {
                _log.Debug($"TLS connection failed for host {mxRecordTlsProfile.MxHostname}");

                string failedConnectionErrors = mxRecordTlsProfile.ConnectionResults.GetFailedConnectionErrors();

                return _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile,
                    EvaluatorResults.GetConnectionFailedResults(failedConnectionErrors));
            }

            if (mxRecordTlsProfile.ConnectionResults.HostNotFound())
            {
                _log.Debug($"Host not found for {mxRecordTlsProfile.MxHostname}");

                return _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile,
                    EvaluatorResults.GetHostNotFoundResults(mxRecordTlsProfile.MxHostname));
            }

            _log.Debug($"Evaluating TLS connection results for {mxRecordTlsProfile.MxHostname}.");

            return _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile,
                _mxSecurityEvaluator.Evaluate(mxRecordTlsProfile.ConnectionResults));

        }
    }
}
