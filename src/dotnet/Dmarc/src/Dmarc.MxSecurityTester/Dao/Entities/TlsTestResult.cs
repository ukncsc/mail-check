using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class TlsTestResult
    {
        public TlsTestResult(TlsVersion? version,
            CipherSuite? cipherSuite,
            CurveGroup? curveGroup,
            SignatureHashAlgorithm? signatureHashAlgorithm,
            Error? error,
            string errorDescription,
            List<string> smtpResponses)
        {
            Version = version;
            CipherSuite = cipherSuite;
            CurveGroup = curveGroup;
            SignatureHashAlgorithm = signatureHashAlgorithm;
            Error = error;
            ErrorDescription = errorDescription;
            SmtpResponses = smtpResponses;
        }

        public TlsVersion? Version { get; }
        public CipherSuite? CipherSuite { get; }
        public CurveGroup? CurveGroup { get; }
        public SignatureHashAlgorithm? SignatureHashAlgorithm { get; }
        public Error? Error { get; }
        public string ErrorDescription { get; }
        public List<string> SmtpResponses { get; }

        protected bool Equals(TlsTestResult other)
        {
            return Version == other.Version &&
                   CipherSuite == other.CipherSuite &&
                   CurveGroup == other.CurveGroup &&
                   SignatureHashAlgorithm == other.SignatureHashAlgorithm &&
                   Error == other.Error &&
                   string.Equals(ErrorDescription, other.ErrorDescription) &&
                   Equals(SmtpResponses, other.SmtpResponses);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TlsTestResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Version.GetHashCode();
                hashCode = (hashCode * 397) ^ CipherSuite.GetHashCode();
                hashCode = (hashCode * 397) ^ CurveGroup.GetHashCode();
                hashCode = (hashCode * 397) ^ SignatureHashAlgorithm.GetHashCode();
                hashCode = (hashCode * 397) ^ Error.GetHashCode();
                hashCode = (hashCode * 397) ^ (ErrorDescription != null ? ErrorDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SmtpResponses != null ? SmtpResponses.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}