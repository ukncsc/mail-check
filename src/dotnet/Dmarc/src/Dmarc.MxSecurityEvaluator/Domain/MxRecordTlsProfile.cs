using Dmarc.Common.Interface.Tls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.MxSecurityEvaluator.Domain
{
    public class MxRecordTlsProfile
    {
        public MxRecordTlsProfile(int mxRecordId, string mxHostname, DateTime lastChecked, ConnectionResults connectionResults)
        {
            MxRecordId = mxRecordId;
            MxHostname = mxHostname;
            LastChecked = lastChecked;
            ConnectionResults = connectionResults;
        }

        public int MxRecordId { get; }

        public string MxHostname { get; }

        public DateTime LastChecked { get; }

        public ConnectionResults ConnectionResults;
    }
}
