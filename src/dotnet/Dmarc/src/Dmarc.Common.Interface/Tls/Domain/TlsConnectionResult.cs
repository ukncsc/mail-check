using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Dmarc.Common.Interface.Tls.Domain
{
    public class TlsConnectionResult
    {
        public TlsConnectionResult(Error error) 
            : this(null, null, null, null, null, error)
        {}

        public TlsConnectionResult(TlsVersion? version, 
            CipherSuite? cipherSuite,
            CurveGroup? curveGroup, 
            SignatureHashAlgorithm? signatureHashAlgorithm,
            List<X509Certificate2> certificates,
            Error? error)
        {
            Version = version;
            CipherSuite = cipherSuite;
            CurveGroup = curveGroup;
            SignatureHashAlgorithm = signatureHashAlgorithm;
            Certificates = certificates ?? new List<X509Certificate2>();
            Error = error;
        }

        public TlsVersion? Version { get; }
        public CipherSuite? CipherSuite { get; }
        public CurveGroup? CurveGroup { get; }
        public SignatureHashAlgorithm? SignatureHashAlgorithm { get; }
        public List<X509Certificate2> Certificates { get; }
        public Error? Error { get; }

        public override string ToString()
        {
            string certs = string.Join(Environment.NewLine, Certificates.Select((_, i) => $"{i + 1}\ti: {_.Issuer}{Environment.NewLine}\ts: {_.Subject}"));

            return $"{nameof(Version)}: {Version}{Environment.NewLine}" +
                   $"{nameof(CipherSuite)}: {CipherSuite}{Environment.NewLine}" +
                   $"{nameof(CurveGroup)}: {CurveGroup}{Environment.NewLine}" +
                   $"{nameof(SignatureHashAlgorithm)}: {SignatureHashAlgorithm}{Environment.NewLine}" +
                   $"{nameof(Certificates)}:{Environment.NewLine}{certs}{Environment.NewLine}" +
                   $"{nameof(Error)}: {Error}";
        }
    }
}