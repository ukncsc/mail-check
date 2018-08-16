using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace Dmarc.Common.Interface.Tls.Domain
{
    public class TlsConnectionResult
    {
        public TlsConnectionResult(Error error, string errorDescription, List<string> smtpResponses) 
            : this(null, null, null, null, error, errorDescription, smtpResponses)
        {}


        [JsonConstructor]
        public TlsConnectionResult(TlsVersion? version,
            CipherSuite? cipherSuite,
            CurveGroup? curveGroup,
            SignatureHashAlgorithm? signatureHashAlgorithm,
            Error? error,
            string errorDescription,
            List<string> smtpResponses
        ) : this(version, cipherSuite, curveGroup, signatureHashAlgorithm, error, errorDescription, smtpResponses, null)
        {

        }

        public TlsConnectionResult(TlsVersion? version, 
            CipherSuite? cipherSuite,
            CurveGroup? curveGroup, 
            SignatureHashAlgorithm? signatureHashAlgorithm,
            Error? error,
            string errorDescription,
            List<string> smtpResponses,
            List<X509Certificate2> certificates = null
            )
        {
            Version = version;
            CipherSuite = cipherSuite;
            CurveGroup = curveGroup;
            SignatureHashAlgorithm = signatureHashAlgorithm;
            Certificates = certificates ?? new List<X509Certificate2>();
            Error = error;
            ErrorDescription = errorDescription;
            SmtpResponses = smtpResponses;
        }

        public TlsVersion? Version { get; }
        public CipherSuite? CipherSuite { get; }
        public CurveGroup? CurveGroup { get; }
        public SignatureHashAlgorithm? SignatureHashAlgorithm { get; }
        [JsonIgnore]
        public List<X509Certificate2> Certificates { get; }
        public Error? Error { get; }
        public string ErrorDescription { get; }
        public List<string> SmtpResponses { get; }

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