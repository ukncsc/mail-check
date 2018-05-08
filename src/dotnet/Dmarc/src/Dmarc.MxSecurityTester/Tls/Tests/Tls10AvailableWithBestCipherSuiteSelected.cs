using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityTester.Tls.Tests
{
    public class Tls10AvailableWithBestCipherSuiteSelected : Tls11AvailableWithBestCipherSuiteSelected
    {
        public override int Id => 8;

        public override string Name => nameof(Tls10AvailableWithBestCipherSuiteSelected);

        public override TlsVersion Version => TlsVersion.TlsV1;
    }
}