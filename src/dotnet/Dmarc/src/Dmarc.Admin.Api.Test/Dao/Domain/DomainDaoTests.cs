using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.Domain;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Dao.Domain
{
    [TestFixture(Category = "Integration")]
    public class DomainDaoTests : DatabaseTestBase
    {
        private const string Domain1 = "test1.domain.org";
        private const string Domain2 = "test2.domain.org";
        private const string Domain3 = "test3.domain.org";
        private const string Domain4 = "test4.domain.org";
        private const string FirstName = "testFirstName";
        private const string LastName = "testLastName";
        private const string Email = FirstName + "@" + Domain1;
        private const string Group1 = "Test Group1";
        private const string Group2 = "Test Group2";

        private DomainDao _domainDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            IConnectionInfoAsync connectionInfo = A.Fake<IConnectionInfoAsync>();
            _domainDao = new DomainDao(connectionInfo);

            A.CallTo(() => connectionInfo.GetConnectionStringAsync()).Returns(ConnectionString);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task GetDomainByIdReturnsDomainWhenDomainExists()
        {
            int domainId = TestHelpers.CreateDomain(ConnectionString, Domain1);

            Api.Domain.Domain domain = await _domainDao.GetDomainById(domainId);

            Assert.That(domain.Id, Is.EqualTo(domainId));
            Assert.That(domain.Name, Is.EqualTo(Domain1));
        }

        [Test]
        public async Task GetDomainsByIdReturnsNullWhenDomainDoesntExist()
        {
            Api.Domain.Domain domain = await _domainDao.GetDomainById(1);

            Assert.That(domain, Is.Null);
        }

        [Test]
        public async Task CreateDomainCorrectlyInsertsDomain()
        {
            Api.Domain.Domain domain = await _domainDao.CreateDomain(Domain1, 321);

            List<Api.Domain.Domain> domains = TestHelpers.GetAllDomains(ConnectionString);

            Assert.That(domains.Count, Is.EqualTo(1));
            Assert.That(domain, Is.EqualTo(domains[0]));
        }

        [Test]
        public async Task GetDomainsByUserIdReturnsDomainsWhenUserInGroups()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
            });

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group2Id, domain3Id),
                Tuple.Create(group2Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByUserId(userId, string.Empty, 1, 10);
            Assert.That(domains.Count, Is.EqualTo(4));
            Assert.That(domains[0].Id, Is.EqualTo(domain1Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain1));
            Assert.That(domains[1].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain2));
            Assert.That(domains[2].Id, Is.EqualTo(domain3Id));
            Assert.That(domains[2].Name, Is.EqualTo(Domain3));
            Assert.That(domains[3].Id, Is.EqualTo(domain4Id));
            Assert.That(domains[3].Name, Is.EqualTo(Domain4));
        }

        [Test]
        public async Task GetDomainsByUserIdReturnsDomainsWhenUserInGroupsRespectsSearchCriteria()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
            });

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group2Id, domain3Id),
                Tuple.Create(group2Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByUserId(userId, "test2", 1, 10);
            Assert.That(domains.Count, Is.EqualTo(1));
            Assert.That(domains[0].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByUserIdReturnsDomainsWhenUserInGroupsRespectsSearchCriteriaCaseInsensitive()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
            });

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group2Id, domain3Id),
                Tuple.Create(group2Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByUserId(userId, "TeST2", 1, 10);
            Assert.That(domains.Count, Is.EqualTo(1));
            Assert.That(domains[0].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByUserIdReturnsDomainsWhenUserInGroupsRespectsPageCriteria()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
            });

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group2Id, domain3Id),
                Tuple.Create(group2Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByUserId(userId, string.Empty, 2, 2);
            Assert.That(domains.Count, Is.EqualTo(2));
            Assert.That(domains[0].Id, Is.EqualTo(domain3Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain3));
            Assert.That(domains[1].Id, Is.EqualTo(domain4Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain4));
        }

        [Test]
        public async Task GetDomainsByUserIdReturnsDomainsWhenUserInGroupsRespectsPageSizeCriteria()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);

            TestHelpers.CreateGroupUserMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, userId),
                Tuple.Create(group2Id, userId),
            });

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group2Id, domain3Id),
                Tuple.Create(group2Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByUserId(userId, string.Empty, 1, 2);
            Assert.That(domains.Count, Is.EqualTo(2));
            Assert.That(domains[0].Id, Is.EqualTo(domain1Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain1));
            Assert.That(domains[1].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByUserIdReturnsEmptyListWhenUserNotInGroups()
        {
            int userId = TestHelpers.CreateUser(ConnectionString, FirstName, LastName, Email);

            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);
            int group2Id = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group2Id, domain3Id),
                Tuple.Create(group2Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByUserId(userId, string.Empty, 1, 10);
            Assert.That(domains, Is.Empty);
        }

        [Test]
        public async Task GetDomainsByGroupIdReturnsDomainsWhenDomainsInGroups()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group1Id, domain3Id),
                Tuple.Create(group1Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByGroupId(group1Id, string.Empty, 1, 10);
            Assert.That(domains.Count, Is.EqualTo(4));
            Assert.That(domains[0].Id, Is.EqualTo(domain1Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain1));
            Assert.That(domains[1].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain2));
            Assert.That(domains[2].Id, Is.EqualTo(domain3Id));
            Assert.That(domains[2].Name, Is.EqualTo(Domain3));
            Assert.That(domains[3].Id, Is.EqualTo(domain4Id));
            Assert.That(domains[3].Name, Is.EqualTo(Domain4));
        }

        [Test]
        public async Task GetDomainsByGroupIdReturnsDomainsWhenDomainsInGroupsRespectsSearchCriteria()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group1Id, domain3Id),
                Tuple.Create(group1Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByGroupId(group1Id, "test2", 1, 10);
            Assert.That(domains.Count, Is.EqualTo(1));
            Assert.That(domains[0].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByGroupIdReturnsDomainsWhenDomainsInGroupsRespectsSearchCriteriaCaseInsensitive()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group1Id, domain3Id),
                Tuple.Create(group1Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByGroupId(group1Id, "TeSt2", 1, 10);
            Assert.That(domains.Count, Is.EqualTo(1));
            Assert.That(domains[0].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByGroupIdReturnsDomainsWhenDomainsInGroupsRespectsPageCriteria()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group1Id, domain3Id),
                Tuple.Create(group1Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByGroupId(group1Id, string.Empty, 2, 2);
            Assert.That(domains.Count, Is.EqualTo(2));
            Assert.That(domains[0].Id, Is.EqualTo(domain3Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain3));
            Assert.That(domains[1].Id, Is.EqualTo(domain4Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain4));
        }

        [Test]
        public async Task GetDomainsByGroupIdReturnsDomainsWhenDomainsInGroupsRespectsPageSizeCriteria()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            TestHelpers.CreateGroupDomainMapping(ConnectionString, new List<Tuple<int, int>>
            {
                Tuple.Create(group1Id, domain1Id),
                Tuple.Create(group1Id, domain2Id),
                Tuple.Create(group1Id, domain3Id),
                Tuple.Create(group1Id, domain4Id),
            });

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByGroupId(group1Id, string.Empty, 1, 2);
            Assert.That(domains.Count, Is.EqualTo(2));
            Assert.That(domains[0].Id, Is.EqualTo(domain1Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain1));
            Assert.That(domains[1].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByGroupIdReturnsEmptyListWhenUserNotInGroups()
        {
            int group1Id = TestHelpers.CreateGroup(ConnectionString, Group1);

            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByGroupId(group1Id, string.Empty, 1, 2);
            Assert.That(domains, Is.Empty);
        }

        [Test]
        public async Task GetDomainsByNameReturnsMatchingDomainsNoSearchTerm()
        {
            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByName(string.Empty, 10, new List<int>());

            Assert.That(domains.Count, Is.EqualTo(4));
            Assert.That(domains[0].Id, Is.EqualTo(domain1Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain1));
            Assert.That(domains[1].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain2));
            Assert.That(domains[2].Id, Is.EqualTo(domain3Id));
            Assert.That(domains[2].Name, Is.EqualTo(Domain3));
            Assert.That(domains[3].Id, Is.EqualTo(domain4Id));
            Assert.That(domains[3].Name, Is.EqualTo(Domain4));
        }

        [Test]
        public async Task GetDomainsByNameReturnsMatchingDomains()
        {
            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByName("test2", 10, new List<int>());

            Assert.That(domains.Count, Is.EqualTo(1));
            Assert.That(domains[0].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByNameReturnsMatchingDomainsIsCaseInsenstive()
        {
            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByName("TEsT2", 10, new List<int>());

            Assert.That(domains.Count, Is.EqualTo(1));
            Assert.That(domains[0].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByNameReturnsEmptyListWhenNoMatchingDomains()
        {
            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByName(string.Empty, 10, new List<int>());

            Assert.That(domains, Is.Empty);
        }

        [Test]
        public async Task GetDomainsByNameReturnsMatchingDomainsRespectsLimit()
        {
            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByName(string.Empty, 2, new List<int>());

            Assert.That(domains.Count, Is.EqualTo(2));
            Assert.That(domains[0].Id, Is.EqualTo(domain1Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain1));
            Assert.That(domains[1].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain2));
        }

        [Test]
        public async Task GetDomainsByNameReturnsMatchingDomainsRespectsIncludeIds()
        {
            int domain1Id = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domain2Id = TestHelpers.CreateDomain(ConnectionString, Domain2);
            int domain3Id = TestHelpers.CreateDomain(ConnectionString, Domain3);
            int domain4Id = TestHelpers.CreateDomain(ConnectionString, Domain4);

            List<Api.Domain.Domain> domains = await _domainDao.GetDomainsByName(string.Empty, 2, new List<int> { domain3Id });

            Assert.That(domains.Count, Is.EqualTo(3));
            Assert.That(domains[0].Id, Is.EqualTo(domain1Id));
            Assert.That(domains[0].Name, Is.EqualTo(Domain1));
            Assert.That(domains[1].Id, Is.EqualTo(domain2Id));
            Assert.That(domains[1].Name, Is.EqualTo(Domain2));
            Assert.That(domains[2].Id, Is.EqualTo(domain3Id));
            Assert.That(domains[2].Name, Is.EqualTo(Domain3));
        }
    }
}
