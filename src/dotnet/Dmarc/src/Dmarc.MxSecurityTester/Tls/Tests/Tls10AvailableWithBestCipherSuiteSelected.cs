using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Tls.Tests
{
    public class Tls10AvailableWithBestCipherSuiteSelected : Tls11AvailableWithBestCipherSuiteSelected
    {
        public override int Id => (int)TlsTestType.Tls10AvailableWithBestCipherSuiteSelected;

        public override string Name => nameof(Tls10AvailableWithBestCipherSuiteSelected);

        public override TlsVersion Version => TlsVersion.TlsV1;
    }
}