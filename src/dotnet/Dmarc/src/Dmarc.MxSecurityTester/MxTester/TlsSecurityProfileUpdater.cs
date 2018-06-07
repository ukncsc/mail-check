using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityTester.Dao.Entities;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface ITlsSecurityProfileUpdater
    {
        Task<List<DomainTlsSecurityProfile>> UpdateSecurityProfiles(List<DomainTlsSecurityProfile> tlsSecurityProfiles);
    }

    internal class TlsSecurityProfileUpdater : ITlsSecurityProfileUpdater
    {
        private readonly ICachingTlsSecurityTesterAdapator _tlsSecurityTester;
        private readonly ILogger _log;

        public TlsSecurityProfileUpdater(ICachingTlsSecurityTesterAdapator tlsSecurityTester, 
            ILogger log)
        {
            _tlsSecurityTester = tlsSecurityTester;
            _log = log;
        }

        public async Task<List<DomainTlsSecurityProfile>> UpdateSecurityProfiles(List<DomainTlsSecurityProfile> tlsSecurityProfiles)
        {
            List<DomainTlsSecurityProfile> updatedSecurityProfiles = new List<DomainTlsSecurityProfile>();
            foreach (DomainTlsSecurityProfile tlsSecurityProfile in tlsSecurityProfiles)
            {
                updatedSecurityProfiles.Add(await UpdateDomainTlsSecurityProfile(tlsSecurityProfile));
            }
            return updatedSecurityProfiles;
        }

        private async Task<DomainTlsSecurityProfile> UpdateDomainTlsSecurityProfile(DomainTlsSecurityProfile domainTlsSecurityProfile)
        {
            _log.Debug($"Updating TLS Security Profiles for domain: {domainTlsSecurityProfile.Domain.Name}");
            List<MxRecordTlsSecurityProfile> updatedMxRecordTlsSecurityProfiles = new List<MxRecordTlsSecurityProfile>();

            foreach (MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile in domainTlsSecurityProfile.Profiles)
            {
                updatedMxRecordTlsSecurityProfiles.AddRange(await UpdateMxSecurityProfile(mxRecordTlsSecurityProfile));
            }

            return new DomainTlsSecurityProfile(domainTlsSecurityProfile.Domain, updatedMxRecordTlsSecurityProfiles);
        }

        private async Task<List<MxRecordTlsSecurityProfile>> UpdateMxSecurityProfile(MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile)
        {
            MxRecordTlsSecurityProfile updatedSecurityProfile = await _tlsSecurityTester.Test(mxRecordTlsSecurityProfile);

            bool recordUnchanged = mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Equals(updatedSecurityProfile.TlsSecurityProfile.Results);
            bool newRecord = !mxRecordTlsSecurityProfile.TlsSecurityProfile.Id.HasValue;

            _log.Debug($"Updated tls profile (mx record id:{mxRecordTlsSecurityProfile.MxRecord.Id}, " +
                       $"tls profile id:{mxRecordTlsSecurityProfile.TlsSecurityProfile.Id?.ToString() ?? "null"}) " +
                       $"record changed: {!recordUnchanged}, new record: {newRecord}");

            return recordUnchanged || newRecord
                ? new List<MxRecordTlsSecurityProfile> { updatedSecurityProfile }
                : new List<MxRecordTlsSecurityProfile> { CreateExpiredRecord(mxRecordTlsSecurityProfile), CreateNewRecord(updatedSecurityProfile) };
        }

        private MxRecordTlsSecurityProfile CreateExpiredRecord(MxRecordTlsSecurityProfile oldMxRecordTlsSecurityProfile)
        {
            return new MxRecordTlsSecurityProfile(
                oldMxRecordTlsSecurityProfile.MxRecord,
                new TlsSecurityProfile(
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Id,
                DateTime.UtcNow,
                new TlsTestResults(
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.FailureCount,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test1Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test2Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test3Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test4Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test5Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test6Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test7Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test8Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test9Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test10Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test11Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test12Result,
                oldMxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Certificates)));
        }

        private MxRecordTlsSecurityProfile CreateNewRecord(MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile)
        {
            return new MxRecordTlsSecurityProfile(
                mxRecordTlsSecurityProfile.MxRecord,
                new TlsSecurityProfile(
                null,
                null,
                new TlsTestResults(
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.FailureCount,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test1Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test2Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test3Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test4Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test5Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test6Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test7Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test8Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test9Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test10Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test11Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Test12Result,
                mxRecordTlsSecurityProfile.TlsSecurityProfile.Results.Certificates)));
        }
    }
}