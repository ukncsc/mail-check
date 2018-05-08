using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DomainTlsEvaluatorResults
    {
        public DomainTlsEvaluatorResults(int id, string hostname, List<MxTlsEvaluatorResults> mxTlsEvaluatorResults, bool pending = false)
        {
            Id = id;
            Hostname = hostname;
            MxTlsEvaluatorResults = mxTlsEvaluatorResults;
            Pending = pending;
        }

        public DomainTlsEvaluatorResults(int id, bool pending)
        {
            Id = id;
            Hostname = null;
            MxTlsEvaluatorResults = new List<MxTlsEvaluatorResults>();
            Pending = pending;
        }

        public string Hostname { get; }

        public int Id { get; }

        public List<MxTlsEvaluatorResults> MxTlsEvaluatorResults { get; }

        public bool Pending { get; }
    }
}
