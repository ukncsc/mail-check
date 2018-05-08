using Dmarc.Common.Validation;
using NUnit.Framework;

namespace Dmarc.Common.Test.Validation
{
    [TestFixture]
    public class DomainValidatorTest
    {
        private DomainValidator _domainValidator;

        [SetUp]
        public void SetUp()
        {
            _domainValidator = new DomainValidator();
        }
        
        [TestCase(".ab", false, TestName = "Only tld with . invalid")]
        [TestCase(null, false, TestName = "Null is invalid")]
        [TestCase("", false, TestName = "Empty stirng is invalid")]
        [TestCase("ab", false, TestName = "Only tld invalid")]
        [TestCase("a.b", false, TestName = "tld of 1 invalid")]
        [TestCase("a.ab", true, TestName = "tld of 2 valid")]
        [TestCase("a.c.d.e.f.ab", true, TestName = "subdomains valid")]
        [TestCase("a.11", false, TestName = "number as tld invalid")]
        [TestCase("1.ab", true, TestName = "number as domain valid")]
        [TestCase("www.ab", true, TestName = "www valid")]
        [TestCase("12.-12.ab", false, TestName = "hyphen at start of element invalid")]
        [TestCase("12.ab-.ab", false, TestName = "hyphen at end element invalid")]
        [TestCase("a-b.ab", true, TestName = "hyphen valid")]
        [TestCase("a$.ab", false, TestName = "$ invalid")]
        [TestCase("a@.ab", false, TestName = "@ invalid")]
        [TestCase("a&.ab", false, TestName = "& invalid")]
        [TestCase("a:.ab", false, TestName = ": invalid")]
        [TestCase("a;.ab", false, TestName = ": invalid")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.ab", true, TestName = "domain of 63 valid")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.ab", false, TestName = "domain of 64 invalid")]
        [TestCase("a.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", true, TestName = "tld of 63 valid")]
        [TestCase("a.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", false, TestName = "tld of 64 invalid")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", true, TestName = "length 253 valid")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", false, TestName = "length 254 invalid")]
        public void TestValidDomains(string domain, bool isValid)
        {
            Assert.That(_domainValidator.IsValidDomain(domain), Is.EqualTo(isValid));
        }
    }
}
