using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.AggregateReport.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;


namespace Dmarc.AggregateReport.Api.Test.Validation
{
    [TestFixture]
    internal class DomainSearchRequestValidationTests
    {
        private DomainSearchRequestValidator _domainSearchRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _domainSearchRequestValidator = new DomainSearchRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public async Task Test(DomainSearchRequest domainSearch, bool expectedToBeValid)
        {
            ValidationResult validationResult = await _domainSearchRequestValidator.ValidateAsync(domainSearch);
            Assert.That(validationResult.IsValid, Is.EqualTo(expectedToBeValid));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = null }, false).SetName("null search pattern is invalid");
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = "" }, false).SetName("empty search pattern is invalid");
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = "s"}, true).SetName("not null search pattern is valid");
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = "$"}, false).SetName("illegal char is invalid");
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = "$asdf;576567$$" }, false).SetName("illegal chars with valid chars is invalid");
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = "searchPatternsearchPatternsearchPatternsearchPatte"}, true).SetName("not null search pattern of 50 chars is valid");
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = "searchPatternsearchPatternsearchPatternsearchPatter"}, false).SetName("not null search pattern over 50 chars is  invalid");
            yield return new TestCaseData(new DomainSearchRequest { SearchPattern = "ABC" }, true).SetName("all upper case chars is valid");
        }
    }
}
