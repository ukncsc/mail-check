using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Validation
{
    [TestFixture]
    public class EntitySearchRequestValidatorTests
    {
        private EntitySearchRequestValidator _entitySearchRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _entitySearchRequestValidator = new EntitySearchRequestValidator();  
        }

        [TestCaseSource(nameof(CreateTestData))]
        public async Task Test(EntitySearchRequest request, bool isValid)
        {
            ValidationResult validationResult = await _entitySearchRequestValidator.ValidateAsync(request);
            Assert.That(validationResult.IsValid, Is.EqualTo(isValid));
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new EntitySearchRequest { Search = null }, true).SetName("Null search term valid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = string.Empty }, true).SetName("Empty string search term valid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search" }, true).SetName("Non null search term valid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search$" }, false).SetName("Search term containing $ is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search<" }, false).SetName("Search term containing < is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search>" }, false).SetName("Search term containing > is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search%" }, false).SetName("Search term containing % is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search\r" }, false).SetName("Search term containing \r is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search\n" }, false).SetName("Search term containing \n is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search\\" }, false).SetName("Search term containing \\ is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search/" }, false).SetName("Search term containing / is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "Search;" }, false).SetName("Search term containing ; is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "SearchSearchSearchSearchSearchSearchSearchSearchSe" }, true).SetName("Search term of 50 chars is valid.");
            yield return new TestCaseData(new EntitySearchRequest { Search = "SearchSearchSearchSearchSearchSearchSearchSearchSea" }, false).SetName("Search term of 51 chars is invalid.");
            yield return new TestCaseData(new EntitySearchRequest { Limit = 0}, false).SetName("Limit of 0 invalid");
            yield return new TestCaseData(new EntitySearchRequest { Limit = 1}, true).SetName("Limit of 1 valid");
            yield return new TestCaseData(new EntitySearchRequest { Limit = 200}, true).SetName("Limit of 200 valid");
            yield return new TestCaseData(new EntitySearchRequest { Limit = 201}, false).SetName("Limit of 201 invalid");
            yield return new TestCaseData(new EntitySearchRequest { IncludedIds = new List<int> {1} }, true).SetName("Positive included ids valid");
            yield return new TestCaseData(new EntitySearchRequest { IncludedIds = new List<int> {-1} }, false).SetName("Negative included ids invalid");
        }
    }
}
