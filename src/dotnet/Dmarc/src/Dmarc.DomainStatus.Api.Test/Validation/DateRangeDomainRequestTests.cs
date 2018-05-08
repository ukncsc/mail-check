using System;
using System.Collections.Generic;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.DomainStatus.Api.Test.Validation
{
    [TestFixture]
    public class DateRangeDomainRequestTests
    {
        private DateRangeDomainRequestValidator _dateRangeDomainRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _dateRangeDomainRequestValidator = new DateRangeDomainRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(DateRangeDomainRequest request, bool valid)
        {
            ValidationResult result = _dateRangeDomainRequestValidator.Validate(request);
            Assert.That(result.IsValid, Is.EqualTo(valid));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            DateTime now = DateTime.Now.Date;
            DateTime earlier = DateTime.Now.Date.AddDays(-2);

            yield return new TestCaseData(new DateRangeDomainRequest { Id = 1, StartDate = earlier, EndDate = now}, true).SetName("EndDate greater than StartDate valid.");
            yield return new TestCaseData(new DateRangeDomainRequest { Id = 1, StartDate = earlier, EndDate = earlier}, true).SetName("EndDate equal to StartDate valid.");
            yield return new TestCaseData(new DateRangeDomainRequest { Id = 1, StartDate = now, EndDate = earlier}, false).SetName("EndDate less than StartDate invalid.");
        }
    }
}
