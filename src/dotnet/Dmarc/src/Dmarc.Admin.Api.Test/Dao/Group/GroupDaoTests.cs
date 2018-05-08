using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.Group;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Dao.Group
{
    [TestFixture(Category = "Integration")] 
    public class GroupDaoTests : DatabaseTestBase
    {
        private const string Group1 = "Test Group1";
        private const string Group2 = "Test Group2";
        private const string Group3 = "Test Group3";
        private const string Group4 = "Test Group4";

        private const string FirstName = "testFirstName";
        private const string LastName = "testLastName";
        private const string Email = FirstName + "@" + Domain1;

        private const string Domain1 = "test1.domain.org";

        private GroupDao _groupDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            IConnectionInfoAsync connectionInfo = A.Fake<IConnectionInfoAsync>();
            _groupDao = new GroupDao(connectionInfo);

            A.CallTo(() => connectionInfo.GetConnectionStringAsync()).Returns(ConnectionString);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task GetGroupsByIdReturnsGroupsWhenGroupsExist()
        {
            int groupId = TestHelpers.CreateGroup(ConnectionString, Group1);

            Api.Domain.Group group = await _groupDao.GetGroupById(groupId);

            Assert.That(group.Id, Is.EqualTo(groupId));
            Assert.That(group.Name, Is.EqualTo(Group1));
        }

        [Test]
        public async Task GetGroupsByIdReturnsNullWhenNoGroupExists()
        {
            Api.Domain.Group group = await _groupDao.GetGroupById(1);

            Assert.That(group, Is.Null);
        }

        [Test]
        public async Task CreateGroupCorrectlyInsertsGroup()
        {
            GroupForCreation groupForCreation = new GroupForCreation
            {
                Name = Group1
            };

            Api.Domain.Group group = await _groupDao.CreateGroup(groupForCreation);

            List<Api.Domain.Group> groups = TestHelpers.GetAllGroups(ConnectionString);

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].Id, Is.EqualTo(group.Id));
            Assert.That(groups[0].Name, Is.EqualTo(group.Name));
        }

        [Test]
        public async Task GetGroupsByUserIdReturnsGroupsWhenUserInGroups()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
                Tuple.Create(group3Id, userId),
                Tuple.Create(group4Id, userId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByUserId(userId, string.Empty, 1, 10);

            Assert.That(groups.Count, Is.EqualTo(4));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
            Assert.That(groups[1].Id, Is.EqualTo(group2Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group2));
            Assert.That(groups[2].Id, Is.EqualTo(group3Id));
            Assert.That(groups[2].Name, Is.EqualTo(Group3));
            Assert.That(groups[3].Id, Is.EqualTo(group4Id));
            Assert.That(groups[3].Name, Is.EqualTo(Group4));
        }

        [Test]
        public async Task GetGroupsByUserIdReturnsGroupsWhenUserInGroupsRespectsSearchCriteria()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
                Tuple.Create(group3Id, userId),
                Tuple.Create(group4Id, userId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByUserId(userId, "Test Group1", 1, 10);

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
        }

        [Test]
        public async Task GetGroupsByUserIdReturnsGroupsWhenUserInGroupsRespectsSearchCriteriaCaseInsensitive()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
                Tuple.Create(group3Id, userId),
                Tuple.Create(group4Id, userId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByUserId(userId, "TeSt GrOuP1", 1, 10);

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
        }

        [Test]
        public async Task GetGroupsByUserIdReturnsGroupsWhenUserInGroupsRespectsPageCriteria()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
                Tuple.Create(group3Id, userId),
                Tuple.Create(group4Id, userId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByUserId(userId, string.Empty, 2, 2);

            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(groups[0].Id, Is.EqualTo(group3Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group3));
            Assert.That(groups[1].Id, Is.EqualTo(group4Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group4));
        }

        [Test]
        public async Task GetGroupsByUserIdReturnsGroupsWhenUserInGroupsRespectsPageSizeCriteria()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
                Tuple.Create(group3Id, userId),
                Tuple.Create(group4Id, userId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByUserId(userId, string.Empty, 1, 2);

            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
            Assert.That(groups[1].Id, Is.EqualTo(group2Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group2));
        }

        [Test]
        public async Task GetGroupsByUserIdReturnsEmptyWhenUserNotInGroups()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByUserId(userId, string.Empty, 1, 2);

            Assert.That(groups, Is.Empty);
        }

        [Test]
        public async Task GetGroupsByDomainIdReturnsGroupsWhenDomainInGroups()
        {
            int domainId = TestHelpers.CreateDomain(ConnectionString, Domain1);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domainId),
                Tuple.Create(group2Id, domainId),
                Tuple.Create(group3Id, domainId),
                Tuple.Create(group4Id, domainId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByDomainId(domainId, string.Empty, 1, 10);

            Assert.That(groups.Count, Is.EqualTo(4));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
            Assert.That(groups[1].Id, Is.EqualTo(group2Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group2));
            Assert.That(groups[2].Id, Is.EqualTo(group3Id));
            Assert.That(groups[2].Name, Is.EqualTo(Group3));
            Assert.That(groups[3].Id, Is.EqualTo(group4Id));
            Assert.That(groups[3].Name, Is.EqualTo(Group4));
        }

        [Test]
        public async Task GetGroupsByDomainIdReturnsGroupsWhenDomainInGroupsRespectsSearchCriteria()
        {
            int domainId = TestHelpers.CreateDomain(ConnectionString, Domain1);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domainId),
                Tuple.Create(group2Id, domainId),
                Tuple.Create(group3Id, domainId),
                Tuple.Create(group4Id, domainId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByDomainId(domainId, "Test Group1", 1, 10);

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
        }

        [Test]
        public async Task GetGroupsByDomainIdReturnsGroupsWhenDomainInGroupsRespectsSearchCriteriaCaseInsensitive()
        {
            int domainId = TestHelpers.CreateDomain(ConnectionString, Domain1);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domainId),
                Tuple.Create(group2Id, domainId),
                Tuple.Create(group3Id, domainId),
                Tuple.Create(group4Id, domainId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByDomainId(domainId, "TeSt GrOuP1", 1, 10);

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
        }

        [Test]
        public async Task GetGroupsByDomainIdReturnsGroupsWhenDomainInGroupsRespectsPageCriteria()
        {
            int domainId = TestHelpers.CreateDomain(ConnectionString, Domain1);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domainId),
                Tuple.Create(group2Id, domainId),
                Tuple.Create(group3Id, domainId),
                Tuple.Create(group4Id, domainId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByDomainId(domainId, string.Empty, 2, 2);

            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(groups[0].Id, Is.EqualTo(group3Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group3));
            Assert.That(groups[1].Id, Is.EqualTo(group4Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group4));
        }

        [Test]
        public async Task GetGroupsByDomainIdReturnsGroupsWhenDomainInGroupsRespectsPageSizeCriteria()
        {
            int domainId = TestHelpers.CreateDomain(ConnectionString, Domain1);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domainId),
                Tuple.Create(group2Id, domainId),
                Tuple.Create(group3Id, domainId),
                Tuple.Create(group4Id, domainId),
            });

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByDomainId(domainId, string.Empty, 1, 2);

            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
            Assert.That(groups[1].Id, Is.EqualTo(group2Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group2));
        }

        [Test]
        public async Task GetGroupsByDomainIdReturnsEmptyWhenDomainNotInGroups()
        {
            int domainId = TestHelpers.CreateDomain(ConnectionString, Domain1);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByDomainId(domainId, string.Empty, 1, 2);

            Assert.That(groups, Is.Empty);
        }

        [Test]
        public async Task GetGroupsByNameReturnsMatchingGroupsNoSearchTerm()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByName(string.Empty, 10, new List<int>());

            Assert.That(groups.Count, Is.EqualTo(4));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
            Assert.That(groups[1].Id, Is.EqualTo(group2Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group2));
            Assert.That(groups[2].Id, Is.EqualTo(group3Id));
            Assert.That(groups[2].Name, Is.EqualTo(Group3));
            Assert.That(groups[3].Id, Is.EqualTo(group4Id));
            Assert.That(groups[3].Name, Is.EqualTo(Group4));
        }

        [Test]
        public async Task GetGroupsByNameReturnsMatchingGroups()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByName("Test Group1", 10, new List<int>());

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
        }

        [Test]
        public async Task GetGroupsByNameReturnsMatchingGroupsIsCaseInsenstive()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByName("TeSt GrOuP1", 10, new List<int>());

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
        }

        [Test]
        public async Task GetGroupsByNameReturnsEmptyListWhenNoMatchingGroups()
        {
            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByName(string.Empty, 10, new List<int>());

            Assert.That(groups, Is.Empty);
        }

        [Test]
        public async Task GetGroupsByNameReturnsMatchingGroupsRespectsLimit()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByName(string.Empty, 2, new List<int>());

            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
            Assert.That(groups[1].Id, Is.EqualTo(group2Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group2));
        }

        [Test]
        public async Task GetGroupsByNameReturnsMatchingGroupsRespectsIncludeIds()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);
            int group3Id = TestHelpers.CreateGroup(ConnectionString, Group3);
            int group4Id = TestHelpers.CreateGroup(ConnectionString, Group4);

            List<Api.Domain.Group> groups = await _groupDao.GetGroupsByName(string.Empty, 2, new List<int> {group3Id});

            Assert.That(groups.Count, Is.EqualTo(3));
            Assert.That(groups[0].Id, Is.EqualTo(group1Id));
            Assert.That(groups[0].Name, Is.EqualTo(Group1));
            Assert.That(groups[1].Id, Is.EqualTo(group2Id));
            Assert.That(groups[1].Name, Is.EqualTo(Group2));
            Assert.That(groups[2].Id, Is.EqualTo(group3Id));
            Assert.That(groups[2].Name, Is.EqualTo(Group3));
        }
    }
}
