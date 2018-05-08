using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.GroupDomain;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Dao.GroupDomain
{
    [TestFixture(Category = "Integration")]
    public class GroupDomainDaoTests : DatabaseTestBase
    {
        private const string Group1 = "Test Group1";
        private const string Group2 = "Test Group2";

        private const string Domain1 = "test1.domain.org";
        private const string Domain2 = "test2.domain.org";

        private GroupDomainDao _groupDomainDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            IConnectionInfoAsync connectionInfo = A.Fake<IConnectionInfoAsync>();
            _groupDomainDao = new GroupDomainDao(connectionInfo);

            A.CallTo(() => connectionInfo.GetConnectionStringAsync()).Returns(ConnectionString);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddGroupDomainsCorrectlyAddsGroupsToDomains()
        {
            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            List<Tuple<int, int>> groupDomains = new List<Tuple<int, int>>
            {
                Tuple.Create(groupId1, domainId1),
                Tuple.Create(groupId1, domainId2),
                Tuple.Create(groupId2, domainId1),
                Tuple.Create(groupId2, domainId2)
            };

            await _groupDomainDao.AddGroupDomains(groupDomains);

            List<Tuple<int, int>> groupDomainsFromDb = TestHelpers.GetAllGroupDomains(ConnectionString);

            Assert.That(groupDomains.SequenceEqual(groupDomainsFromDb), Is.True);
        }

        [Test]
        public async Task DeleteGroupDomainsCorrectlyDeletesGroupsFromDomains()
        {
            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            List<Tuple<int, int>> groupDomains = new List<Tuple<int, int>>
            {
                Tuple.Create(groupId1, domainId1),
                Tuple.Create(groupId1, domainId2),
                Tuple.Create(groupId2, domainId1),
                Tuple.Create(groupId2, domainId2)
            };

            TestHelpers.CreateGroupDomainMapping(ConnectionString, groupDomains);

            await _groupDomainDao.DeleteGroupDomains(groupDomains);

            List<Tuple<int, int>> groupDomainsFromDb = TestHelpers.GetAllGroupDomains(ConnectionString);

            Assert.That(groupDomainsFromDb, Is.Empty);
        }
    }
}
