using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class TlsTestResult
    {
        public TlsTestResult(TlsVersion? version,
            CipherSuite? cipherSuite,
            CurveGroup? curveGroup,
            SignatureHashAlgorithm? signatureHashAlgorithm,
            Error? error)
        {
            Version = version;
            CipherSuite = cipherSuite;
            CurveGroup = curveGroup;
            SignatureHashAlgorithm = signatureHashAlgorithm;
            Error = error;
        }

        public TlsVersion? Version { get; }
        public CipherSuite? CipherSuite { get; }
        public CurveGroup? CurveGroup { get; }
        public SignatureHashAlgorithm? SignatureHashAlgorithm { get; }
        public Error? Error { get; }

        protected bool Equals(TlsTestResult other)
        {
            return Version == other.Version && 
                CipherSuite == other.CipherSuite && 
                CurveGroup == other.CurveGroup && 
                SignatureHashAlgorithm == other.SignatureHashAlgorithm && 
                Error == other.Error;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
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
                return hashCode;
            }
        }
    }
}