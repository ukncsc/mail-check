using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Tls.BouncyCastle.KeyExchange;
using Dmarc.Common.Tls.BouncyCastle.Mapping;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using CipherSuite = Dmarc.Common.Interface.Tls.Domain.CipherSuite;

namespace Dmarc.Common.Tls.BouncyCastle
{
    internal class TestTlsClientProtocol : TlsClientProtocol
    {
        private Error? _error;
        private string _errorMessage;

        public TestTlsClientProtocol(Stream stream, SecureRandom secureRandom)
            : base(stream, secureRandom)
        {
        }

        public TestTlsClientProtocol(Stream input, Stream output, SecureRandom secureRandom)
            : base(input, output, secureRandom)
        {
        }

        public TestTlsClientProtocol(SecureRandom secureRandom)
            : base(secureRandom)
        {
        }

        public TestTlsClientProtocol(Stream input)
            : this(input, SecureRandom.GetInstance("SHA256PRNG"))
        {
        }

        public TlsConnectionResult ConnectWithResults(Org.BouncyCastle.Crypto.Tls.TlsClient tlsClient)
        {
            try
            {
                Connect(tlsClient);
            }
            catch (TlsFatalAlertReceived e)
            {
                _error = (Error) e.AlertDescription;
                _errorMessage = e.Message;
                return new TlsConnectionResult(_error.Value, e.Message, null);
            }
            catch (TlsFatalAlert e)
            {
                _error = (Error) e.AlertDescription;
                _errorMessage = e.Message;
                return new TlsConnectionResult(_error.Value, e.Message, null);
            }
            catch (Exception e)
            {
                _error = Error.INTERNAL_ERROR;
                _errorMessage = e.Message;
                return new TlsConnectionResult(_error.Value, e.Message, null);
            }

            switch (mKeyExchange.GetType().Name)
            {
                case nameof(TestTlsDheKeyExchange):
                    return ProcessKeyExchange((TestTlsDheKeyExchange)mKeyExchange);

                case nameof(TestTlsDhKeyExchange):
                    return ProcessKeyExchange((TestTlsDhKeyExchange)mKeyExchange);

                case nameof(TestTlsEcDheKeyExchange):
                    return ProcessKeyExchange((TestTlsEcDheKeyExchange)mKeyExchange);

                case nameof(TestTlsEcDhKeyExchange):
                    return ProcessKeyExchange((TestTlsEcDhKeyExchange)mKeyExchange);

                case nameof(TlsRsaKeyExchange):
                    return ProcessKeyExchange((TlsRsaKeyExchange)mKeyExchange);

                default:
                    throw new InvalidOperationException($"{mKeyExchange.GetType()} is not recognised key exchange.");
            }
        }

        private TlsConnectionResult ProcessKeyExchange(TestTlsDheKeyExchange keyExchange)
        {
            CurveGroup group = keyExchange.DhParameters.ToGroup();

            TlsVersion version = Context.ServerVersion.ToTlsVersion();
            CipherSuite cipherSuite = mSecurityParameters.CipherSuite.ToCipherSuite();
            SignatureHashAlgorithm signatureHashAlgorithm = keyExchange.EcSignatureAndHashAlgorithm.ToSignatureAlgorithm();
            List<X509Certificate2> certificates = mPeerCertificate.ToCertificateList();

            base.CleanupHandshake();
            return new TlsConnectionResult(version, cipherSuite, group, signatureHashAlgorithm, _error, _errorMessage, null, certificates);
        }

        private TlsConnectionResult ProcessKeyExchange(TestTlsDhKeyExchange keyExchange)
        {
            CurveGroup group = keyExchange.DhParameters.ToGroup();

            TlsVersion version = Context.ServerVersion.ToTlsVersion();
            CipherSuite cipherSuite = mSecurityParameters.CipherSuite.ToCipherSuite();
            List<X509Certificate2> certificates = mPeerCertificate.ToCertificateList();

            base.CleanupHandshake();
            return new TlsConnectionResult(version, cipherSuite, group, null, _error, _errorMessage, null, certificates);
        }

        private TlsConnectionResult ProcessKeyExchange(TestTlsEcDheKeyExchange keyExchange)
        {
            string curveName = keyExchange.EcPublicKeyParameters.Parameters.Curve.GetType().Name.ToLower();

            CurveGroup curve = curveName.ToCurve();
            TlsVersion version = Context.ServerVersion.ToTlsVersion();
            CipherSuite cipherSuite = mSecurityParameters.CipherSuite.ToCipherSuite();
            SignatureHashAlgorithm signatureHashAlgorithm = keyExchange.EcSignatureAndHashAlgorithm.ToSignatureAlgorithm();
            List<X509Certificate2> certificates = mPeerCertificate.ToCertificateList();

            base.CleanupHandshake();
            return new TlsConnectionResult(version, cipherSuite, curve, signatureHashAlgorithm, _error, _errorMessage, null, certificates);
        }

        private TlsConnectionResult ProcessKeyExchange(TestTlsEcDhKeyExchange keyExchange)
        {
            string curveName = keyExchange.EcPublicKeyParameters.Parameters.Curve.GetType().Name.ToLower();

            CurveGroup curve = curveName.ToCurve();
            TlsVersion version = Context.ServerVersion.ToTlsVersion();
            CipherSuite cipherSuite = mSecurityParameters.CipherSuite.ToCipherSuite();
            List<X509Certificate2> certificates = mPeerCertificate.ToCertificateList();

            base.CleanupHandshake();
            return new TlsConnectionResult(version, cipherSuite, curve, null,  _error, _errorMessage, null, certificates);
        }

        private TlsConnectionResult ProcessKeyExchange(TlsRsaKeyExchange keyExchange)
        {
            TlsVersion version = Context.ServerVersion.ToTlsVersion();
            CipherSuite cipherSuite = mSecurityParameters.CipherSuite.ToCipherSuite();
            List<X509Certificate2> certificates = mPeerCertificate.ToCertificateList();

            base.CleanupHandshake();
            return new TlsConnectionResult(version, cipherSuite, null, null,  _error, _errorMessage, null, certificates);
        }

        protected override void CleanupHandshake() { }
    }
}