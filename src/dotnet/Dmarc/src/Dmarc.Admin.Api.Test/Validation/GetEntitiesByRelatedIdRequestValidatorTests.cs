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
    public class GetEntitiesByRelatedIdRequestValidatorTests
    {
        private GetEntitiesByRelatedIdRequestValidator _getEntitiesByRelatedIdRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _getEntitiesByRelatedIdRequestValidator = new GetEntitiesByRelatedIdRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestData))]
        public async Task Test(GetEntitiesByRelatedIdRequest request, bool isValid)
        {
            ValidationResult validationResult = await _getEntitiesByRelatedIdRequestValidator.ValidateAsync(request);
            Assert.That(validationResult.IsValid, Is.EqualTo(isValid));
        }

        private static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1}, true).SetName("Positive id and default values is valid");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = -1}, false).SetName("Negative id is invalid");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Page = 0}, false).SetName("Page less than 1 is invalid");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, PageSize = 0 }, false).SetName("PageSize less than 1 is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = null }, true).SetName("Null search term valid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = string.Empty }, true).SetName("Empty string search term valid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search" }, true).SetName("Non null search term valid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search$" }, false).SetName("Search term containing $ is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search<" }, false).SetName("Search term containing < is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search>" }, false).SetName("Search term containing > is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search%" }, false).SetName("Search term containing % is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search\r" }, false).SetName("Search term containing \r is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search\n" }, false).SetName("Search term containing \n is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search\\" }, false).SetName("Search term containing \\ is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search/" }, false).SetName("Search term containing / is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "Search;" }, false).SetName("Search term containing ; is invalid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "SearchSearchSearchSearchSearchSearchSearchSearchSe" }, true).SetName("Search term of 50 chars is valid.");
            yield return new TestCaseData(new GetEntitiesByRelatedIdRequest {Id = 1, Search = "SearchSearchSearchSearchSearchSearchSearchSearchSea" }, false).SetName("Search term of 51 chars is invalid.");
        }
    }
}
