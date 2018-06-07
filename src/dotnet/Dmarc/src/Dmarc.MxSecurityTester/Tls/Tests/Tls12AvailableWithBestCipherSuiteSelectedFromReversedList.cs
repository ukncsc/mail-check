using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Tls.Tests
{
    public class Tls12AvailableWithBestCipherSuiteSelectedFromReversedList :
        Tls12AvailableWithBestCipherSuiteSelected
    {
        public override int Id => (int)TlsTestType.Tls12AvailableWithBestCipherSuiteSelectedFromReverseList;

        public override string Name => nameof(Tls12AvailableWithBestCipherSuiteSelectedFromReversedList);

        public override List<CipherSuite> CipherSuites => Enumerable.Reverse(base.CipherSuites).ToList();
    }
}