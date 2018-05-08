using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityTester.Tls.Tests
{
    public class Tls10AvailableWithWeakCipherSuiteNotSelected : Tls11AvailableWithWeakCipherSuiteNotSelected
    {
        public override int Id => 9;

        public override string Name => nameof(Tls10AvailableWithWeakCipherSuiteNotSelected);

        public override TlsVersion Version => TlsVersion.TlsV1;
    }
}