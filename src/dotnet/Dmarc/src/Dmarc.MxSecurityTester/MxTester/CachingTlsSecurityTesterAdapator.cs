using System;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityTester.Caching;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Dao.Entities;
using Newtonsoft.Json;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface ICachingTlsSecurityTesterAdapator : ITlsSecurityTesterAdapator { }

    internal class CachingTlsSecurityTesterAdapator : ICachingTlsSecurityTesterAdapator
    {
        private const int FailureCountBeforeCaching = 3;
        private const double RefreshIntervalSecondsMultiplier = 0.9;
        private const string KeyPrefix = "MxRecordTlsSecurityProfile4";

        private readonly ITlsSecurityTesterAdapator _tlsSecurityTesterAdaptor;
        private readonly ICache _cache;
        private readonly IMxSecurityTesterConfig _config;
        private readonly ILogger _log;
        
        public CachingTlsSecurityTesterAdapator(ITlsSecurityTesterAdapator tlsSecurityTesterAdaptor,
            ICache cache,
            IMxSecurityTesterConfig config,
            ILogger log)
        {
            _tlsSecurityTesterAdaptor = tlsSecurityTesterAdaptor;
            _cache = cache;
            _config = config;
            _log = log;
        }

        public async Task<MxRecordTlsSecurityProfile> Test(MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile)
        {
            if (_config.CachingEnabled)
            {
                string cachedStringResult = await _cache.GetString($"{KeyPrefix}-{mxRecordTlsSecurityProfile.MxRecord.Hostname?.ToLower()}");

                if (cachedStringResult != null)
                {
                    _log.Debug($"Successfully retrieved TLSSecurityProfile from cache for host {mxRecordTlsSecurityProfile.MxRecord.Hostname}");
                    TlsTestResults cachedResults = JsonConvert.DeserializeObject<TlsTestResults>(cachedStringResult);
                    return new MxRecordTlsSecurityProfile(mxRecordTlsSecurityProfile.MxRecord, 
                        new TlsSecurityProfile(mxRecordTlsSecurityProfile.TlsSecurityProfile.Id, null, cachedResults));
                }
            }

            MxRecordTlsSecurityProfile result = await _tlsSecurityTesterAdaptor.Test(mxRecordTlsSecurityProfile);
            _log.Debug($"Successfully retrieved TLSSecurityProfile from tls tester for host {mxRecordTlsSecurityProfile.MxRecord.Hostname}");

            if (_config.CachingEnabled && (result.TlsSecurityProfile.TlsResults.FailureCount == 0 || result.TlsSecurityProfile.TlsResults.FailureCount >= FailureCountBeforeCaching))
            {
                string resultToCache = JsonConvert.SerializeObject(result.TlsSecurityProfile.TlsResults);

                await _cache.SetString($"{KeyPrefix}-{mxRecordTlsSecurityProfile.MxRecord.Hostname}", resultToCache,
                    TimeSpan.FromSeconds(_config.RefreshIntervalSeconds * RefreshIntervalSecondsMultiplier));

                _log.Debug($"Successfully set TLSSecurityProfile to cache for host {mxRecordTlsSecurityProfile.MxRecord.Hostname}");
            }

            return result;
        }
    }
}
