using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Explainers;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Record;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Spf.Rules.Record
{
    [TestFixture]
    public class ShouldHaveFailAllEnabledTests
    {
        [TestCaseSource(nameof(CreateTestData))]
        public void Test(SpfRecord spfRecord, string expectedError, ErrorType? expectedErrorType)
        {
            ShouldHaveHardFailAllEnabled shouldHaveHardFailAllEnabled = new ShouldHaveHardFailAllEnabled(new QualifierExplainer());

            bool hasError = shouldHaveHardFailAllEnabled.IsErrored(spfRecord, out Error error);

            if (expectedError == null)
            {
                Assert.That(error, Is.Null);
                Assert.That(hasError, Is.False);
            }
            else
            {
                Assert.That(hasError, Is.True);
                Assert.That(error.ErrorType, Is.EqualTo(expectedErrorType.Value));
                Assert.That(error.Message, Is.EqualTo(expectedError));
            }
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new SpfRecord("", new Version(""), new List<Term>(), string.Empty), null, null).SetName("No all term - passes.");
            yield return new TestCaseData(CreateSpfRecord("-all", Qualifier.Fail), null, null).SetName("Fail all - passes.");
            yield return new TestCaseData(CreateSpfRecord("~all", Qualifier.SoftFail), null, null).SetName("Soft Fail all - passes.");
            yield return new TestCaseData(CreateSpfRecord("+all", Qualifier.Pass),
                "Only “-all” (do not allow other ip addresses) or “~all” (allow but mark other ip addresses) protect recipients from spoofed mail. Consider changing from +all (allow other ip addresses) to “-all” or “~all”.",
                ErrorType.Warning).SetName("Pass all - fails.");
            yield return new TestCaseData(CreateSpfRecord("?all", Qualifier.Neutral),
                "Only “-all” (do not allow other ip addresses) or “~all” (allow but mark other ip addresses) protect recipients from spoofed mail. Consider changing from ?all (allow without evaluation other ip addresses) to “-all” or “~all”.",
                ErrorType.Warning).SetName("Neutral all - fails.");
            yield return new TestCaseData(CreateSpfRecord("?all", Qualifier.Pass, true), null, null).SetName("Redirect with all should pass.");
        }

        private static SpfRecord CreateSpfRecord(string value, Qualifier qualifier, bool isRedirect = false)
        {
            All all = new All(value, qualifier);

            if (isRedirect)
            {
                return new SpfRecord("", new Version(""), new List<Term> { all, new Redirect("ncsc.gov.uk", new DomainSpec("ncsc.gov.uk")) }, string.Empty);
            }

            return new SpfRecord("", new Version(""), new List<Term> { all }, string.Empty);
        }
    }
}
