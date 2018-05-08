using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.AggregateReport.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.AggregateReport.Api.Test.Validation
{
    internal class DateRangeDomainValidatorTests
    {
        private DateRangeDomainRequestValidator _dateRangeRequestValidator;

        [SetUp]
        public void Setup()
        {
            _dateRangeRequestValidator = new DateRangeDomainRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public async Task Test(DateRangeDomainRequest dateRangeDomainRequestRequest, bool expectedToBeValid)
        {
            ValidationResult validationResult = await _dateRangeRequestValidator.ValidateAsync(dateRangeDomainRequestRequest);
            Assert.That(validationResult.IsValid, Is.EqualTo(expectedToBeValid));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(new DateRangeDomainRequest { BeginDateUtc = null, EndDateUtc = DateTime.Now, DomainId = null}, false).SetName("null begin date is invalid");
            yield return new TestCaseData(new DateRangeDomainRequest { BeginDateUtc = DateTime.Now, EndDateUtc = null, DomainId = null}, false).SetName("null end date is invalid");
            yield return new TestCaseData(new DateRangeDomainRequest { BeginDateUtc = DateTime.Now, EndDateUtc =DateTime.Now, DomainId = null }, true).SetName("null domain is valid");
        }
    }
}
