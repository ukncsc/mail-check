using Dmarc.Common.Interface.Tls.Domain;
using System;

namespace Dmarc.MxSecurityEvaluator.Util
{
    public static class CipherSuiteUtil
    {
        public static string GetName(this CipherSuite? cipherSuite) =>
            cipherSuite != null
                ? Enum.GetName(typeof(CipherSuite), cipherSuite)
                : "";
    }
}
