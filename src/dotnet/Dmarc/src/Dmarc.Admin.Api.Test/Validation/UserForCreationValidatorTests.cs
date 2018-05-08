using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Validation
{
    [TestFixture]
    public class UserForCreationValidatorTests
    {
        private UserForCreationValidator _userForCreationValidator;

        [SetUp]
        public void SetUp()
        {
            _userForCreationValidator = new UserForCreationValidator();
        }

        [TestCaseSource(nameof(CreateTestData))]
        public async Task Test(UserForCreation user, bool isValid)
        {
            ValidationResult validationResult = await _userForCreationValidator.ValidateAsync(user);
            Assert.That(validationResult.IsValid, Is.EqualTo(isValid));
        }

        private static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = null }, false).SetName("Null FirstName invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = string.Empty }, false).SetName("Empty string FirstName invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName" }, true).SetName("Non null search term valid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName$" }, false).SetName("FirstName containing $ is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName<" }, false).SetName("FirstName containing < is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName>" }, false).SetName("FirstName containing > is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName%" }, false).SetName("FirstName containing % is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName\r" }, false).SetName("FirstName containing \r is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName\n" }, false).SetName("FirstName containing \n is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName\\" }, false).SetName("FirstName containing \\ is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName/" }, false).SetName("FistName containing / is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "FirstName;" }, false).SetName("FistName containing ; is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNam" }, true).SetName("FirstName term of 255 chars is valid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", LastName = "Test", FirstName = "NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName" }, false).SetName("FirstName term of 256 chars is invalid.");

            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = null }, false).SetName("Null LastName invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = string.Empty }, false).SetName("Empty string LastName invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName" }, true).SetName("Non null LastName valid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName$" }, false).SetName("LastName containing $ is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName<" }, false).SetName("LastName containing < is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName>" }, false).SetName("LastName containing > is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName%" }, false).SetName("LastName containing % is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName\r" }, false).SetName("LastName containing \r is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName\n" }, false).SetName("LastName containing \n is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName\\" }, false).SetName("LastName containing \\ is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName/" }, false).SetName("LastName containing / is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "LastName;" }, false).SetName("LastName containing ; is invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNam" }, true).SetName("LastName term of 255 chars is valid.");
            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName" }, false).SetName("LastName term of 256 chars is invalid.");

            yield return new TestCaseData(new UserForCreation { Email = "a@b.com", FirstName = "Test", LastName = "Test" }, true).SetName("Email valid.");
            yield return new TestCaseData(new UserForCreation { Email = "ab.com", FirstName = "Test", LastName = "Test" }, false).SetName("Non Email invalid.");
            yield return new TestCaseData(new UserForCreation { Email = "", FirstName = "Test", LastName = "Test" }, false).SetName("Empty string email invalid.");
            yield return new TestCaseData(new UserForCreation { Email = null, FirstName = "Test", LastName = "Test" }, false).SetName("Null Email invalid.");
        }
    }
}
