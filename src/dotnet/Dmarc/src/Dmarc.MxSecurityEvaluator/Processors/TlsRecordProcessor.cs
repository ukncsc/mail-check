using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls.Domain;
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
        private const int TestCount = 13;

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
                await _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile.MxRecordId,
                    CreateStubResults(TestCount));

                _log.Debug(
                    $"MX record with ID {mxRecordTlsProfile.MxRecordId} has no hostname, saving null results.");
            }
            else if (HasFailedConnection(mxRecordTlsProfile.TlsConnectionResults))
            {
                await _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile.MxRecordId,
                    CreateConnectionFailedResults());

                _log.Debug(
                    $"MX record with ID {mxRecordTlsProfile.MxRecordId} TLS connection failed, saving single inconclusive result.");
            }
            else if (mxRecordTlsProfile.TlsConnectionResults.Count == TestCount)
            {
                var tlsEvaluatorResults = _mxSecurityEvaluator.Evaluate(mxRecordTlsProfile.TlsConnectionResults);

                await _tlsRecordDao.SaveTlsEvaluatorResults(mxRecordTlsProfile.MxRecordId, tlsEvaluatorResults);

                _log.Debug(
                    $"Evaluated TLS connection results for MX record with ID {mxRecordTlsProfile.MxRecordId}.");
            }
        }

        private static bool HasFailedConnection(IEnumerable<TlsConnectionResult> tlsConnectionResults)
        {
            return tlsConnectionResults.All(_ =>
                _.Error == Error.SESSION_INITIALIZATION_FAILED || _.Error == Error.TCP_CONNECTION_FAILED);
        }

        private static List<TlsEvaluatorResult> CreateStubResults(int count, EvaluatorResult? result = null)
        {
            var results = new List<TlsEvaluatorResult>();

            for (var i = 0; i < count; i++)
            {
                results.Add(new TlsEvaluatorResult(result));
            }

            return results;
        }

        private static List<TlsEvaluatorResult> CreateConnectionFailedResults()
        {
            var results = new List<TlsEvaluatorResult>()
            {
                new TlsEvaluatorResult(EvaluatorResult.INCONCLUSIVE,
                    "We were unable to create a TLS connection with this server. This could be because the server does not support " +
                    "TLS or because Mail Check servers have been blocked. We will keep trying to test TLS with this server, " +
                    "so please check back later or get in touch if you think there's a problem.")
            };

            return results.Concat(CreateStubResults(TestCount - 1, EvaluatorResult.PASS)).ToList();
        }
    }
}
