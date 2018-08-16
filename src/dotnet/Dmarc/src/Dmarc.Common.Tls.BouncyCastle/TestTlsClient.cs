using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Tls.BouncyCastle.KeyExchange;
using Dmarc.Common.Tls.BouncyCastle.Mapping;
using Org.BouncyCastle.Crypto.Tls;
using CipherSuite = Dmarc.Common.Interface.Tls.Domain.CipherSuite;

namespace Dmarc.Common.Tls.BouncyCastle
{
    internal class TestTlsClient : DefaultTlsClient
    {
        private readonly TlsVersion _version;
        private readonly List<CipherSuite> _cipherSuites;
        private readonly List<CurveGroup> _supportedGroups;
        private readonly int[] _namedCurves;

        private readonly List<CurveGroup> _defaultSupportedGroups = Enum.GetValues(typeof(CurveGroup))
            .Cast<CurveGroup>()
            .Where(_ => _ <= CurveGroup.Ffdhe8192)
            .ToList();

        public TestTlsClient(TlsVersion version, List<CipherSuite> cipherSuites, List<CurveGroup> supportedGroups = null)
        {
            _version = version;
            _cipherSuites = cipherSuites;
            _supportedGroups = supportedGroups ?? _defaultSupportedGroups;
            _namedCurves = _supportedGroups.Select(_ => (int)_).ToArray();
        }

        public override TlsAuthentication GetAuthentication()
        {
            return new EmptyTlsAuthentication();
        }

        public override ProtocolVersion ClientVersion => _version.ToProtocolVersion();

        public override ProtocolVersion MinimumVersion => _version.ToProtocolVersion();

        public override IDictionary GetClientExtensions()
        {
            IDictionary clientExtensions = base.GetClientExtensions() ?? new Dictionary<object, object>();

            //Switches off SCSV auto adding of SCSV
            if (!_cipherSuites.Contains(CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV))
            {
                clientExtensions.Add(ExtensionType.renegotiation_info, new byte[] {0x00});
            }

            //Remove existing supported groups and add our own
            clientExtensions.Remove(ExtensionType.supported_groups);

            byte[] length = GetBytes((ushort)(_supportedGroups.Count * 2));
            byte[] values = _supportedGroups.SelectMany(_ => GetBytes((ushort)_)).ToArray();

            clientExtensions.Add(ExtensionType.supported_groups, length.Concat(values).ToArray());

            return clientExtensions;
        }

        private byte[] GetBytes(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes;
        }

        public override int[] GetCipherSuites()
        {
            return _cipherSuites.Select(_ => (int)_).ToArray();
        }

        protected override TlsKeyExchange CreateDheKeyExchange(int keyExchange)
        {
            return new TestTlsDheKeyExchange(keyExchange, mSupportedSignatureAlgorithms, null);
        }

        protected override TlsKeyExchange CreateDHKeyExchange(int keyExchange)
        {
            return new TestTlsDhKeyExchange(keyExchange, mSupportedSignatureAlgorithms, null);
        }

        protected override TlsKeyExchange CreateECDheKeyExchange(int keyExchange)
        {
            return new TestTlsEcDheKeyExchange(keyExchange, mSupportedSignatureAlgorithms,
                _namedCurves, mClientECPointFormats, mServerECPointFormats);
        }

        protected override TlsKeyExchange CreateECDHKeyExchange(int keyExchange)
        {
            return new TestTlsEcDhKeyExchange(keyExchange, mSupportedSignatureAlgorithms,
                _namedCurves, mClientECPointFormats, mServerECPointFormats);
        }

        public override void NotifySecureRenegotiation(bool secureRenegotiation){}
    }
}