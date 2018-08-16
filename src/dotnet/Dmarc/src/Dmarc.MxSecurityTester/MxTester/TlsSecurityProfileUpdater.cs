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

            bool recordUnchanged = mxRecordTlsSecurityProfile.TlsSecurityProfile.TlsResults.Equals(updatedSecurityProfile.TlsSecurityProfile.TlsResults);
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
                    oldMxRecordTlsSecurityProfile.TlsSecurityProfile.TlsResults.Clone()));
        }

        private MxRecordTlsSecurityProfile CreateNewRecord(MxRecordTlsSecurityProfile mxRecordTlsSecurityProfile)
        {
            return new MxRecordTlsSecurityProfile(
                mxRecordTlsSecurityProfile.MxRecord,
                new TlsSecurityProfile(
                    null,
                    null, mxRecordTlsSecurityProfile.TlsSecurityProfile.TlsResults.Clone()));
        }
    }
}