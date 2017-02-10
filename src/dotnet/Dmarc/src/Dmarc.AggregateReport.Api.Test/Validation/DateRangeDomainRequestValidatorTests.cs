using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Messages;
using Dmarc.AggregateReport.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.AggregateReport.Api.Test.Validation
{
    internal class DateRangeDomainRequestValidatorTests
    {
        private DateRangeDomainRequestValidator _dateRangeRequestValidator;

        [SetUp]
        public void Setup()
        {
            _dateRangeRequestValidator = new DateRangeDomainRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public async Task Test(DateRangeDomainRequest dateRangeDomainRequest, bool expectedToBeValid)
        {
            ValidationResult validationResult = await _dateRangeRequestValidator.ValidateAsync(dateRangeDomainRequest);
            Assert.That(validationResult.IsValid, Is.EqualTo(expectedToBeValid));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(new DateRangeDomainRequest(null, DateTime.Now, null), false).SetName("null begin date is invalid");
            yield return new TestCaseData(new DateRangeDomainRequest(DateTime.Now, null, null), false).SetName("null end date is invalid");
            yield return new TestCaseData(new DateRangeDomainRequest(DateTime.Now, DateTime.Now, null), true).SetName("null domain is valid");
        }
    }
}
