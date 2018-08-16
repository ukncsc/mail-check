using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.MxSecurityEvaluator.Test.Util
{
    [TestFixture]
    public class CipherSuiteUtilTests
    {
        [Test]
        public void ItShouldReturnTheNameOfANonNullCipherSuite()
        {
            CipherSuite? cipherSuite = CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA;

            Assert.AreEqual(cipherSuite.GetName(), "TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA");
        }

        [Test]
        public void ItShouldReturnAnEmptyStringForANullCipherSuite()
        {
            CipherSuite? cipherSuite = null;

            Assert.AreEqual(cipherSuite.GetName(), "");
        }
    }
}
