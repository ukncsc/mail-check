using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.User;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Dao.User
{
    [TestFixture(Category = "Integration")]
    public class UserDaoTests : DatabaseTestBase
    {
        private const string Domain1 = "test1.domain.org";
        private const string Domain2 = "test2.domain.org";
        private const string Domain3 = "test3.domain.org";
        private const string Domain4 = "test4.domain.org";

        private const string FirstName1 = "test1FirstName";
        private const string LastName1 = "test1LastName";
        private const string Email1 = "user1@" + Domain1;

        private const string FirstName2 = "test2FirstName";
        private const string LastName2 = "test2LastName";
        private const string Email2 = "user2@" + Domain2;

        private const string FirstName3 = "test3FirstName";
        private const string LastName3 = "test3LastName";
        private const string Email3 = "user2@" + Domain3;

        private const string FirstName4 = "test4FirstName";
        private const string LastName4 = "test4LastName";
        private const string Email4 = "user2@" + Domain4;

        private const string Group1 = "Test Group1";

        private UserDao _userDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            IConnectionInfoAsync connectionInfo = A.Fake<IConnectionInfoAsync>();
            _userDao = new UserDao(connectionInfo);

            A.CallTo(() => connectionInfo.GetConnectionStringAsync()).Returns(ConnectionString);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task GetUserByIdReturnsUserWhenUserExists()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);

            Api.Domain.User user = await _userDao.GetUserById(userId);

            Assert.That(user.Id, Is.EqualTo(userId));
            Assert.That(user.FirstName, Is.EqualTo(FirstName1));
            Assert.That(user.LastName, Is.EqualTo(LastName1));
            Assert.That(user.Email, Is.EqualTo(Email1));
        }

        [Test]
        public async Task GetUserByIdReturnsNullUserWhenUserDoesntExist()
        {
            Api.Domain.User user = await _userDao.GetUserById(1);

            Assert.That(user, Is.Null);
        }

        [Test]
        public async Task CreateUserCorrectlyInsertsUser()
        {
            UserForCreation userForCreation = new UserForCreation
            {
                FirstName = FirstName1,
                LastName = LastName1,
                Email = Email1
            };

            Api.Domain.User user = await _userDao.CreateUser(userForCreation);

            List<Api.Domain.User> users = TestHelpers.GetAllUsers(ConnectionString);

            Assert.That(users.Count, Is.EqualTo(1));
            Assert.That(user, Is.EqualTo(users[0]));
        }

        [Test]
        public async Task GetUsersByGroupIdReturnsUsersWhenUserInGroups()
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByGroupId(groupId, string.Empty, 1, 10);
            Assert.That(users.Count, Is.EqualTo(4));
            Assert.That(users[0].Id, Is.EqualTo(userId1));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(users[0].LastName, Is.EqualTo(LastName1));
            Assert.That(users[0].Email, Is.EqualTo(Email1));
            Assert.That(users[1].Id, Is.EqualTo(userId2));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName2));
            Assert.That(users[1].LastName, Is.EqualTo(LastName2));
            Assert.That(users[1].Email, Is.EqualTo(Email2));
            Assert.That(users[2].Id, Is.EqualTo(userId3));
            Assert.That(users[2].FirstName, Is.EqualTo(FirstName3));
            Assert.That(users[2].LastName, Is.EqualTo(LastName3));
            Assert.That(users[2].Email, Is.EqualTo(Email3));
            Assert.That(users[3].Id, Is.EqualTo(userId4));
            Assert.That(users[3].FirstName, Is.EqualTo(FirstName4));
            Assert.That(users[3].LastName, Is.EqualTo(LastName4));
            Assert.That(users[3].Email, Is.EqualTo(Email4));
        }

        [TestCase("test1Fir", true, TestName = "GetUsersByGroupId - First name start with matches")]
        [TestCase("TEsT1fIR", true, TestName = "GetUsersByGroupId - First name start with is case insensitive matches")]
        [TestCase("est1Fir", false, TestName = "GetUsersByGroupId - First name contains with doesnt match")]
        [TestCase("test1Las", true, TestName = "GetUsersByGroupId - Last name start with matches")]
        [TestCase("TEsT1lAs", true, TestName = "GetUsersByGroupId - Last name start with is case insensitive matches")]
        [TestCase("est1Las", false, TestName = "GetUsersByGroupId - Last name contains with doesnt match")]
        [TestCase("ser1@", true, TestName = "GetUsersByGroupId - Email contains matches")]
        [TestCase("SeR1@", true, TestName = "GetUsersByGroupId - Email contains is case insensitive matches")]
        public async Task GetUsersByGroupIdReturnsUsersWhenUserInGroupsRespectSearchCriteria(string search, bool match)
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByGroupId(groupId, search, 1, 10);

            if (match)
            {
                Assert.That(users.Count, Is.EqualTo(1));
                Assert.That(users[0].Id, Is.EqualTo(userId1));
                Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
                Assert.That(users[0].LastName, Is.EqualTo(LastName1));
                Assert.That(users[0].Email, Is.EqualTo(Email1));
            }
            else
            {
                Assert.That(users, Is.Empty);
            }
        }

        [Test]
        public async Task GetUsersByGroupIdReturnsUsersWhenUserInGroupsRespectPageCriteria()
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByGroupId(groupId, string.Empty, 2, 2);

            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users[0].Id, Is.EqualTo(userId3));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName3));
            Assert.That(users[0].LastName, Is.EqualTo(LastName3));
            Assert.That(users[0].Email, Is.EqualTo(Email3));
            Assert.That(users[1].Id, Is.EqualTo(userId4));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName4));
            Assert.That(users[1].LastName, Is.EqualTo(LastName4));
            Assert.That(users[1].Email, Is.EqualTo(Email4));
        }

        [Test]
        public async Task GetUsersByGroupIdReturnsUsersWhenUserInGroupsRespectPageSizeCriteria()
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByGroupId(groupId, string.Empty, 1, 2);

            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users[0].Id, Is.EqualTo(userId1));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(users[0].LastName, Is.EqualTo(LastName1));
            Assert.That(users[0].Email, Is.EqualTo(Email1));
            Assert.That(users[1].Id, Is.EqualTo(userId2));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName2));
            Assert.That(users[1].LastName, Is.EqualTo(LastName2));
            Assert.That(users[1].Email, Is.EqualTo(Email2));
        }

        [Test]
        public async Task GetUsersByGetUsersByDomainIdReturnsUsersWhenUserInGroups()
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
          
            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, domainId1)
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByDomainId(domainId1, string.Empty, 1, 10);
            Assert.That(users.Count, Is.EqualTo(4));
            Assert.That(users[0].Id, Is.EqualTo(userId1));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(users[0].LastName, Is.EqualTo(LastName1));
            Assert.That(users[0].Email, Is.EqualTo(Email1));
            Assert.That(users[1].Id, Is.EqualTo(userId2));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName2));
            Assert.That(users[1].LastName, Is.EqualTo(LastName2));
            Assert.That(users[1].Email, Is.EqualTo(Email2));
            Assert.That(users[2].Id, Is.EqualTo(userId3));
            Assert.That(users[2].FirstName, Is.EqualTo(FirstName3));
            Assert.That(users[2].LastName, Is.EqualTo(LastName3));
            Assert.That(users[2].Email, Is.EqualTo(Email3));
            Assert.That(users[3].Id, Is.EqualTo(userId4));
            Assert.That(users[3].FirstName, Is.EqualTo(FirstName4));
            Assert.That(users[3].LastName, Is.EqualTo(LastName4));
            Assert.That(users[3].Email, Is.EqualTo(Email4));
        }

        [TestCase("test1Fir", true, TestName = "GetUsersByDomainId - First name start with matches")]
        [TestCase("TEsT1fIR", true, TestName = "GetUsersByDomainId - First name start with is case insensitive matches")]
        [TestCase("est1Fir", false, TestName = "GetUsersByDomainId - First name contains with doesnt match")]
        [TestCase("test1Las", true, TestName = "GetUsersByDomainId - Last name start with matches")]
        [TestCase("TEsT1lAs", true, TestName = "GetUsersByDomainId - Last name start with is case insensitive matches")]
        [TestCase("est1Las", false, TestName = "GetUsersByDomainId - Last name contains with doesnt match")]
        [TestCase("ser1@", true, TestName = "GetUsersByDomainId - Email contains matches")]
        [TestCase("SeR1@", true, TestName = "GetUsersByDomainId - Email contains is case insensitive matches")]
        public async Task GetUsersByDomainIdReturnsUsersWhenUserInGroupsRespectSearchCriteria(string search, bool match)
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, domainId1)
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByDomainId(domainId1, search, 1, 10);

            if (match)
            {
                Assert.That(users.Count, Is.EqualTo(1));
                Assert.That(users[0].Id, Is.EqualTo(userId1));
                Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
                Assert.That(users[0].LastName, Is.EqualTo(LastName1));
                Assert.That(users[0].Email, Is.EqualTo(Email1));
            }
            else
            {
                Assert.That(users, Is.Empty);
            }
        }

        [Test]
        public async Task GetUsersByDomainIdReturnsUsersWhenUserInGroupsRespectPageCriteria()
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, domainId1)
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByDomainId(domainId1, string.Empty, 2, 2);

            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users[0].Id, Is.EqualTo(userId3));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName3));
            Assert.That(users[0].LastName, Is.EqualTo(LastName3));
            Assert.That(users[0].Email, Is.EqualTo(Email3));
            Assert.That(users[1].Id, Is.EqualTo(userId4));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName4));
            Assert.That(users[1].LastName, Is.EqualTo(LastName4));
            Assert.That(users[1].Email, Is.EqualTo(Email4));
        }

        [Test]
        public async Task GetUsersByDomainIdReturnsUsersWhenUserInGroupsRespectPageSizeCriteria()
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, userId1),
                Tuple.Create(groupId, userId2),
                Tuple.Create(groupId, userId3),
                Tuple.Create(groupId, userId4),
            });

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(groupId, domainId1)
            });

            List<Api.Domain.User> users = await _userDao.GetUsersByDomainId(domainId1, string.Empty, 1, 2);

            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users[0].Id, Is.EqualTo(userId1));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(users[0].LastName, Is.EqualTo(LastName1));
            Assert.That(users[0].Email, Is.EqualTo(Email1));
            Assert.That(users[1].Id, Is.EqualTo(userId2));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName2));
            Assert.That(users[1].LastName, Is.EqualTo(LastName2));
            Assert.That(users[1].Email, Is.EqualTo(Email2));
        }

        [TestCase("test1Fir", true, TestName = "GetUserByFirstNameLastNameEmail - First name start with matches")]
        [TestCase("TEsT1fIR", true, TestName = "GetUserByFirstNameLastNameEmail - First name start with is case insensitive matches")]
        [TestCase("est1Fir", false, TestName = "GetUserByFirstNameLastNameEmail - First name contains with doesnt match")]
        [TestCase("test1Las", true, TestName = "GetUserByFirstNameLastNameEmail - Last name start with matches")]
        [TestCase("TEsT1lAs", true, TestName = "GetUserByFirstNameLastNameEmail - Last name start with is case insensitive matches")]
        [TestCase("est1Las", false, TestName = "GetUserByFirstNameLastNameEmail - Last name contains with doesnt match")]
        [TestCase("ser1@", true, TestName = "GetUserByFirstNameLastNameEmail - Email contains matches")]
        [TestCase("SeR1@", true, TestName = "GetUserByFirstNameLastNameEmail - Email contains is case insensitive matches")]
        public async Task GetUserByFirstNameLastNameEmailReturnsMatchingDomains(string search, bool match)
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            List<Api.Domain.User> users = await _userDao.GetUsersByFirstNameLastNameEmail(search, 10, new List<int>());

            if (match)
            {
                Assert.That(users.Count, Is.EqualTo(1));
                Assert.That(users[0].Id, Is.EqualTo(userId1));
                Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
                Assert.That(users[0].LastName, Is.EqualTo(LastName1));
                Assert.That(users[0].Email, Is.EqualTo(Email1));
            }
            else
            {
                Assert.That(users, Is.Empty);
            }
        }

        [Test]
        public async Task GetUserByFirstNameLastNameEmailReturnsMatchingDomainsRespectsLimit()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            List<Api.Domain.User> users = await _userDao.GetUsersByFirstNameLastNameEmail(string.Empty, 2, new List<int>());

            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users[0].Id, Is.EqualTo(userId1));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(users[0].LastName, Is.EqualTo(LastName1));
            Assert.That(users[0].Email, Is.EqualTo(Email1));
            Assert.That(users[1].Id, Is.EqualTo(userId2));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName2));
            Assert.That(users[1].LastName, Is.EqualTo(LastName2));
            Assert.That(users[1].Email, Is.EqualTo(Email2));
        }

        [Test]
        public async Task GetUserByFirstNameLastNameEmailReturnsMatchingDomainsRespectsIncludeIds()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);
            int userId3 = TestHelpers.CreateUser(ConnectionString, FirstName3, LastName3, Email3);
            int userId4 = TestHelpers.CreateUser(ConnectionString, FirstName4, LastName4, Email4);

            List<Api.Domain.User> users = await _userDao.GetUsersByFirstNameLastNameEmail(string.Empty, 2, new List<int> {userId3});

            Assert.That(users.Count, Is.EqualTo(3));
            Assert.That(users[0].Id, Is.EqualTo(userId1));
            Assert.That(users[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(users[0].LastName, Is.EqualTo(LastName1));
            Assert.That(users[0].Email, Is.EqualTo(Email1));
            Assert.That(users[1].Id, Is.EqualTo(userId2));
            Assert.That(users[1].FirstName, Is.EqualTo(FirstName2));
            Assert.That(users[1].LastName, Is.EqualTo(LastName2));
            Assert.That(users[1].Email, Is.EqualTo(Email2));
            Assert.That(users[2].Id, Is.EqualTo(userId3));
            Assert.That(users[2].FirstName, Is.EqualTo(FirstName3));
            Assert.That(users[2].LastName, Is.EqualTo(LastName3));
            Assert.That(users[2].Email, Is.EqualTo(Email3));
        }
    }
}
