using System.Collections.Generic;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.DomainStatus.Api.Test.Validation
{
    [TestFixture]
    public class DomainsRequestValidatorTests
    {
        private DomainsRequestValidator _domainsRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _domainsRequestValidator = new DomainsRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestCaseData))]
        public void Test(DomainsRequest request, bool valid)
        {
            ValidationResult result = _domainsRequestValidator.Validate(request);
            Assert.That(result.IsValid, Is.EqualTo(valid));
        }

        public static IEnumerable<TestCaseData> CreateTestCaseData()
        {
            yield return new TestCaseData(new DomainsRequest {Page = null, PageSize = null, Search = string.Empty}, false).SetName("Page and page size null invalid");
            yield return new TestCaseData(new DomainsRequest {Page = 1, PageSize = null, Search = string.Empty}, false).SetName("Page size null invalid");
            yield return new TestCaseData(new DomainsRequest {Page = null, PageSize = 1, Search = string.Empty}, false).SetName("Page null invalid");
            yield return new TestCaseData(new DomainsRequest {Page = 0, PageSize = 1, Search = string.Empty}, false).SetName("Page less than 1 invalid");
            yield return new TestCaseData(new DomainsRequest {Page = 1, PageSize = 0, Search = string.Empty}, false).SetName("Page size less than 1 invalid");
            yield return new TestCaseData(new DomainsRequest {Page = 1, PageSize = 201, Search = string.Empty}, false).SetName("Page size larger than 200 invalid");
            yield return new TestCaseData(new DomainsRequest {Page = 1, PageSize = 200, Search = string.Empty}, true).SetName("Page size of 200 valid");
            yield return new TestCaseData(new DomainsRequest {Page = 1, PageSize = 1, Search = string.Empty}, true).SetName("Page and Page Size valid");
            yield return new TestCaseData(new DomainsRequest { Page = 1, PageSize = 200, Search = "s" }, true).SetName("not null search pattern is valid");
            yield return new TestCaseData(new DomainsRequest { Page = 1, PageSize = 200, Search = "$" }, false).SetName("illegal char is invalid");
            yield return new TestCaseData(new DomainsRequest { Page = 1, PageSize = 200, Search = "$asdf;576567$$" }, false).SetName("illegal chars with valid chars is invalid");
            yield return new TestCaseData(new DomainsRequest { Page = 1, PageSize = 200, Search = "searchPatternsearchPatternsearchPatternsearchPatte" }, true).SetName("not null search pattern of 50 chars is valid");
            yield return new TestCaseData(new DomainsRequest { Page = 1, PageSize = 200, Search = "searchPatternsearchPatternsearchPatternsearchPatter" }, false).SetName("not null search pattern over 50 chars is  invalid");
            yield return new TestCaseData(new DomainsRequest { Page = 1, PageSize = 200, Search = "ABC" }, true).SetName("all upper case chars is valid");
        }
    }
}
