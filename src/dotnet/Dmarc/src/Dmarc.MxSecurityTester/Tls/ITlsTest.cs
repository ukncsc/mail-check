using System.Collections.Generic;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.MxSecurityTester.Tls
{
    public interface ITlsTest
    {
        int Id { get; }
        string Name { get; }
        TlsVersion Version { get; }
        List<CipherSuite> CipherSuites { get; }
    }
}
