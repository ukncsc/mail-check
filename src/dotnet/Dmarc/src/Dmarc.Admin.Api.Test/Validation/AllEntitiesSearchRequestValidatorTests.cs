using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Validation
{
    [TestFixture]
    public class AllEntitiesSearchRequestValidatorTests
    {
        private AllEntitiesSearchRequestValidator _allEntitiesSearchRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _allEntitiesSearchRequestValidator = new AllEntitiesSearchRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestData))]
        public async Task Test(AllEntitiesSearchRequest request, bool isValid)
        {
            ValidationResult validationResult = await _allEntitiesSearchRequestValidator.ValidateAsync(request);
            Assert.That(validationResult.IsValid, Is.EqualTo(isValid));
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = null }, true).SetName("Null search term valid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = string.Empty }, true).SetName("Empty string search term valid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search" }, true).SetName("Non null search term valid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search$" }, false).SetName("Search term containing $ is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search<" }, false).SetName("Search term containing < is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search>" }, false).SetName("Search term containing > is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search%" }, false).SetName("Search term containing % is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search\r" }, false).SetName("Search term containing \r is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search\n" }, false).SetName("Search term containing \n is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search\\" }, false).SetName("Search term containing \\ is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search/" }, false).SetName("Search term containing / is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "Search;" }, false).SetName("Search term containing ; is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "SearchSearchSearchSearchSearchSearchSearchSearchSe" }, true).SetName("Search term of 50 chars is valid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Search = "SearchSearchSearchSearchSearchSearchSearchSearchSea" }, false).SetName("Search term of 51 chars is invalid.");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Limit = 0 }, false).SetName("Limit of 0 invalid");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Limit = 1 }, true).SetName("Limit of 1 valid");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Limit = 200 }, true).SetName("Limit of 200 valid");
            yield return new TestCaseData(new AllEntitiesSearchRequest { Limit = 201 }, false).SetName("Limit of 201 invalid");
        }
    }
}
