using Dmarc.Common.Interface.Tls.Domain;
using System.Collections.Generic;

namespace Dmarc.MxSecurityEvaluator.Domain
{
    public class MxRecordTlsProfile
    {
        public MxRecordTlsProfile(int mxRecordId, string mxHostname, List<TlsConnectionResult> tlsConnectionResults)
        {
            MxRecordId = mxRecordId;
            MxHostname = mxHostname;
            TlsConnectionResults = tlsConnectionResults;
        }

        public int MxRecordId { get; }

        public string MxHostname { get; }

        public List<TlsConnectionResult> TlsConnectionResults;
    }
}
