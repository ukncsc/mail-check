using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using Dmarc.Common.Validation;

namespace Dmarc.Admin.Api.Test.Validation
{
    public class PublicDomainValidatorTests
    {
        private PublicDomainValidator _publicDomainValidator;

        [SetUp]
        public void SetUp()
        {
            _publicDomainValidator = new PublicDomainValidator(new DomainValidator(), new PublicDomainListValidator());
        }

        [TestCaseSource(nameof(CreateTestData))]
        public async Task Test(PublicDomainForCreation request, bool isValid)
        {
            ValidationResult validationResult = await _publicDomainValidator.ValidateAsync(request);
            Assert.That(validationResult.IsValid, Is.EqualTo(isValid));
        }
        
        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new PublicDomainForCreation { Name = null }, false).SetName("A \"domain\" field is required. - fail");
            yield return new TestCaseData(new PublicDomainForCreation { Name = string.Empty }, false).SetName("A \"domain\" field is required. - fail");
            yield return new TestCaseData(new PublicDomainForCreation { Name = "abc.com" }, false).SetName("The domain name must be a public domain name abc.com. - fail");
            yield return new TestCaseData(new PublicDomainForCreation { Name = "abcgov.uk" }, false).SetName("The domain name must be a public domain name abcgov.uk. - fail");
            yield return new TestCaseData(new PublicDomainForCreation { Name = "gov.uk" }, false).SetName("TLD will fail gov.uk - fail");
            yield return new TestCaseData(new PublicDomainForCreation { Name = "abc.gov.uk" }, true).SetName("Valid public domain abc.gov.uk - succeed");
        }

    }
}
