using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.GroupUser;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Dao.GroupUser
{
    [TestFixture(Category = "Integration")]
    public class GroupUserDaoTests : DatabaseTestBase
    {
        private const string Group1 = "Test Group1";
        private const string Group2 = "Test Group2";

        private const string FirstName1 = "testFirstName1";
        private const string LastName1 = "testLastName1";
        private const string Email1 = FirstName1 + "@" + "test.domain.org";

        private const string FirstName2 = "testFirstName2";
        private const string LastName2 = "testLastName2";
        private const string Email2 = FirstName2 + "@" + "test.domain.org";

        private GroupUserDao _groupUserDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            IConnectionInfoAsync connectionInfo = A.Fake<IConnectionInfoAsync>();
            _groupUserDao = new GroupUserDao(connectionInfo);

            A.CallTo(() => connectionInfo.GetConnectionStringAsync()).Returns(ConnectionString);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddGroupUsersCorrectlyAddsGroupsToUsers()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            List<Tuple<int, int>> groupUsers = new List<Tuple<int, int>>
            {
                Tuple.Create(groupId1, userId1),
                Tuple.Create(groupId1, userId2),
                Tuple.Create(groupId2, userId1),
                Tuple.Create(groupId2, userId2)
            };

            await _groupUserDao.AddGroupUsers(groupUsers);

            List<Tuple<int, int>> groupDomainsFromDb = TestHelpers.GetAllGroupUsers(ConnectionString);

            Assert.That(groupUsers.SequenceEqual(groupDomainsFromDb), Is.True);
        }

        [Test]
        public async Task DeleteGroupUsersCorrectlyDeletesGroupsFromUsers()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            List<Tuple<int, int>> groupUsers = new List<Tuple<int, int>>
            {
                Tuple.Create(groupId1, userId1),
                Tuple.Create(groupId1, userId2),
                Tuple.Create(groupId2, userId1),
                Tuple.Create(groupId2, userId2)
            };

            TestHelpers.CreateGroupUserMapping(ConnectionString, groupUsers);

            await _groupUserDao.DeleteGroupUsers(groupUsers);

            List<Tuple<int, int>> groupUsersFromDb = TestHelpers.GetAllGroupUsers(ConnectionString);

            Assert.That(groupUsersFromDb, Is.Empty);
        }
    }
}
