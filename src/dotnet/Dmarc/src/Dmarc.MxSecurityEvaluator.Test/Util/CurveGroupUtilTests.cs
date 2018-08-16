using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.MxSecurityEvaluator.Util;
using NUnit.Framework;

namespace Dmarc.MxSecurityEvaluator.Test.Util
{
    [TestFixture]
    public class CurveGroupUtilTests
    {
        [Test]
        public void ItShouldReturnTheGroupNameOfANonNullCurveGroup()
        {
            CurveGroup? curveGroup = CurveGroup.Ffdhe2048;

            Assert.AreEqual(curveGroup.GetGroupName(), "Ffdhe2048");
        }

        [Test]
        public void ItShouldReturnAnEmptyStringForANullCurveGroup()
        {
            CurveGroup? curveGroup = null;

            Assert.AreEqual(curveGroup.GetGroupName(), "");
        }
    }
}
