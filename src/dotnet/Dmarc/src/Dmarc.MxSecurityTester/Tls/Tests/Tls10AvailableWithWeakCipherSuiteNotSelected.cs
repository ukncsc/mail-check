using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Tls.Tests
{
    public class Tls10AvailableWithWeakCipherSuiteNotSelected : Tls11AvailableWithWeakCipherSuiteNotSelected
    {
        public override int Id => (int)TlsTestType.Tls10AvailableWithWeakCipherSuiteNotSelected;

        public override string Name => nameof(Tls10AvailableWithWeakCipherSuiteNotSelected);

        public override TlsVersion Version => TlsVersion.TlsV1;
    }
}