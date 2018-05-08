using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Validation
{
    [TestFixture]
    public class ChangeMembershipRequestValidatorTest
    {
        private ChangeMembershipRequestValidator _changeMembershipRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _changeMembershipRequestValidator = new ChangeMembershipRequestValidator();
        }

        [TestCaseSource(nameof(CreateTestData))]
        public async Task Test(ChangeMembershipRequest request, bool isValid)
        {
            ValidationResult validationResult = await _changeMembershipRequestValidator.ValidateAsync(request);
            Assert.That(validationResult.IsValid, Is.EqualTo(isValid));
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new ChangeMembershipRequest { Id = 1, EntityIds = new List<int> {1,2,3}}, true).SetName("Positive id and postive non empty entity ids is valid");
            yield return new TestCaseData(new ChangeMembershipRequest { Id = -1, EntityIds = new List<int> {1,2,3}}, false).SetName("Negative id is invalid");
            yield return new TestCaseData(new ChangeMembershipRequest { Id = 1, EntityIds = new List<int> {-1,2,3}}, false).SetName("Negative entity id is invalid");
            yield return new TestCaseData(new ChangeMembershipRequest { Id = 1, EntityIds = new List<int>()}, false).SetName("Empty entity id list is invalid");
        }
    }
}
