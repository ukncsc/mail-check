using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Messages;
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
        public async Task Test(DomainSearchRequest domainSearchRequest, bool expectedToBeValid)
        {
            ValidationResult validationResult = await _domainSearchRequestValidator.ValidateAsync(domainSearchRequest);
            Assert.That(validationResult.IsValid, Is.EqualTo(expectedToBeValid));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(new DomainSearchRequest(null), false).SetName("null search pattern is invalid");
            yield return new TestCaseData(new DomainSearchRequest(""), false).SetName("empty search pattern is invalid");
            yield return new TestCaseData(new DomainSearchRequest("s"), true).SetName("not null search pattern is valid");
            yield return new TestCaseData(new DomainSearchRequest("searchPatternsearchPatternsearchPatternsearchPatte"), true).SetName("not null search pattern of 50 chars is valid");
            yield return new TestCaseData(new DomainSearchRequest("searchPatternsearchPatternsearchPatternsearchPatter"), false).SetName("not null search pattern over 50 chars is  invalid");
        }
    }
}
