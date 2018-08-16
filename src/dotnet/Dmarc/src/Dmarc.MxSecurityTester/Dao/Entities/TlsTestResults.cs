using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Dmarc.MxSecurityTester.Tls.Tests;
using Dmarc.MxSecurityTester.Util;
using Newtonsoft.Json;

namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class TlsTestResults
    {
        public TlsTestResults(
            int failureCount,
            TlsTestResultsWithoutCertificate results,
            List<X509Certificate2> certificates)
        {
            FailureCount = failureCount;
            Results = results;
            Certificates = certificates ?? new List<X509Certificate2>();
        }

        protected bool Equals(TlsTestResults other)
        {
            return Results.Equals(other.Results) && Certificates.SequenceEqual(other.Certificates);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TlsTestResults) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Results != null ? Results.GetHashCode() : 0) * 397) ^ (Certificates != null ? Certificates.GetHashCode() : 0);
            }
        }
        
        public int FailureCount { get; }
        public TlsTestResultsWithoutCertificate Results { get; }

        [JsonConverter(typeof(X509CertificateConverter))]
        public List<X509Certificate2> Certificates { get; }

        public TlsTestResults Clone()
        {
            return new TlsTestResults(FailureCount, Results, Certificates);
        }

    }
}