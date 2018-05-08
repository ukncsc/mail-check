using System.Threading.Tasks;
using Dmarc.Admin.Api.Domain;
using Dmarc.Admin.Api.Validation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Validation
{
    [TestFixture]
    public class GroupForCreationValidatorTests
    {
        private GroupForCreationValidator _groupForCreationValidator;

        [SetUp]
        public void SetUp()
        {
            _groupForCreationValidator = new GroupForCreationValidator();    
        }

        [TestCase(null, false, TestName = "Null string is invalid group name")]
        [TestCase("", false, TestName = "Empty string is invalid group name")]
        [TestCase("Group", true, TestName = "Non null string is valid group name")]
        [TestCase("Group$", false, TestName = "Dollar invalid in group name")]
        [TestCase("Group<", false, TestName = "Left Angle bracket invalid in group name")]
        [TestCase("Group>", false, TestName = "Right Angle bracket invalid in group name")]
        [TestCase("Group%", false, TestName = "Percent invalid in group name")]
        [TestCase("Group\r", false, TestName = "Carriage return invalid in group name")]
        [TestCase("Group\n", false, TestName = "Line feed invalid in group name")]
        [TestCase("Group\\", false, TestName = "Back slash invalid in group name")]
        [TestCase("Group/", false, TestName = "Forward slash invalid in group name")]
        [TestCase("Group;", false, TestName = "Semi colon invalid in group name")]
        [TestCase("GroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroup", true, TestName = "Group name 255 chars long valid")]
        [TestCase("GroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupGroupG", false, TestName = "Group name longer than 255 chars invalid")]
        public async Task Test(string groupName, bool isValid)
        {
            GroupForCreation group = new GroupForCreation
            {
                Name = groupName
            };

            ValidationResult validationResult = await _groupForCreationValidator.ValidateAsync(group);

            Assert.That(validationResult.IsValid, Is.EqualTo(isValid));
        }
    }
}
