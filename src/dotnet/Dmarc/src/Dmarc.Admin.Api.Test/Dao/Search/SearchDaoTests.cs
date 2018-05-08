using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.Search;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Dao.Search
{
    [TestFixture(Category = "Integration")]
    public class SearchDaoTests : DatabaseTestBase
    {
        private const string FirstName1 = "test1FirstName";
        private const string LastName1 = "test1LastName";
        private const string Email1 = "User1@test1.domain.org";

        private const string FirstName2 = "test2FirstName";
        private const string LastName2 = "test2LastName";
        private const string Email2 = "User2@test2.domain.org";

        private const string Group1 = "Test3 Group";
        private const string Group2 = "Test4 Group";

        private const string Domain1 = "test5.domain.org";
        private const string Domain2 = "test6.domain.org";

        private SearchDao _searchDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            IConnectionInfoAsync connectionInfo = A.Fake<IConnectionInfoAsync>();
            _searchDao = new SearchDao(connectionInfo);

            A.CallTo(() => connectionInfo.GetConnectionStringAsync()).Returns(ConnectionString);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenUserNameStartsWithTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("test1F", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.User.Results[0].Id, Is.EqualTo(userId1));
            Assert.That(searchResult.User.Results[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(searchResult.User.Results[0].LastName, Is.EqualTo(LastName1));
            Assert.That(searchResult.User.Results[0].Email, Is.EqualTo(Email1));
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenUserNameStartsWithTermIsCaseInSensitive()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("TeST1f", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.User.Results[0].Id, Is.EqualTo(userId1));
            Assert.That(searchResult.User.Results[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(searchResult.User.Results[0].LastName, Is.EqualTo(LastName1));
            Assert.That(searchResult.User.Results[0].Email, Is.EqualTo(Email1));
        }

        [Test]
        public async Task GetSearchResultsDoesntReturnsResultsWhenUserNameDoesntStartWithTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("1FirstName", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenLastUserNameStartsWithTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("test1L", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.User.Results[0].Id, Is.EqualTo(userId1));
            Assert.That(searchResult.User.Results[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(searchResult.User.Results[0].LastName, Is.EqualTo(LastName1));
            Assert.That(searchResult.User.Results[0].Email, Is.EqualTo(Email1));
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenLastUserNameStartsWithTermIsCaseInsensitive()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("TeSt1l", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.User.Results[0].Id, Is.EqualTo(userId1));
            Assert.That(searchResult.User.Results[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(searchResult.User.Results[0].LastName, Is.EqualTo(LastName1));
            Assert.That(searchResult.User.Results[0].Email, Is.EqualTo(Email1));
        }

        [Test]
        public async Task GetSearchResultsDoesntReturnsResultsWhenUserLastNameDoesntStartWithTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("1LastName", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenLastUserEmailContainsTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("1.domain", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.User.Results[0].Id, Is.EqualTo(userId1));
            Assert.That(searchResult.User.Results[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(searchResult.User.Results[0].LastName, Is.EqualTo(LastName1));
            Assert.That(searchResult.User.Results[0].Email, Is.EqualTo(Email1));
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenUserEmailContainsTermIsCaseInsensitive()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("1.DoMaIn", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.User.Results[0].Id, Is.EqualTo(userId1));
            Assert.That(searchResult.User.Results[0].FirstName, Is.EqualTo(FirstName1));
            Assert.That(searchResult.User.Results[0].LastName, Is.EqualTo(LastName1));
            Assert.That(searchResult.User.Results[0].Email, Is.EqualTo(Email1));
        }

        [Test]
        public async Task GetSearchResultsDoesntReturnsResultsWhenUserEmailDoesntContainTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("7.domain", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenGroupNameContainsTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("est3 Gr", 10);

            Assert.That(searchResult.Group.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.Group.Results[0].Id, Is.EqualTo(groupId1));
            Assert.That(searchResult.Group.Results[0].Name, Is.EqualTo(Group1));
            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenGroupNameContainsTermIsCaseInsenstive()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("EsT3 gr", 10);

            Assert.That(searchResult.Group.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.Group.Results[0].Id, Is.EqualTo(groupId1));
            Assert.That(searchResult.Group.Results[0].Name, Is.EqualTo(Group1));
            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsDoesntReturnsResultsWhenGroupNameDoesntContainsTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("est5 Gr", 10);

            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenDomainNameContainsTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("est5.", 10);

            Assert.That(searchResult.Domain.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.Domain.Results[0].Id, Is.EqualTo(domainId1));
            Assert.That(searchResult.Domain.Results[0].Name, Is.EqualTo(Domain1));
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsReturnsResultsWhenDomainNameContainsTermIsCaseInsensitive()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("EsT5.", 10);

            Assert.That(searchResult.Domain.Results.Count, Is.EqualTo(1));
            Assert.That(searchResult.Domain.Results[0].Id, Is.EqualTo(domainId1));
            Assert.That(searchResult.Domain.Results[0].Name, Is.EqualTo(Domain1));
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }

        [Test]
        public async Task GetSearchResultsDoesntReturnsResultsWhenDomainNameDoesntContainTerm()
        {
            int userId1 = TestHelpers.CreateUser(ConnectionString, FirstName1, LastName1, Email1);
            int userId2 = TestHelpers.CreateUser(ConnectionString, FirstName2, LastName2, Email2);

            int groupId1 = TestHelpers.CreateGroup(ConnectionString, Group1);
            int groupId2 = TestHelpers.CreateGroup(ConnectionString, Group2);

            int domainId1 = TestHelpers.CreateDomain(ConnectionString, Domain1);
            int domainId2 = TestHelpers.CreateDomain(ConnectionString, Domain2);

            SearchResult searchResult = await _searchDao.GetSearchResults("EsT7.", 10);

            Assert.That(searchResult.Domain.Results, Is.Empty);
            Assert.That(searchResult.Group.Results, Is.Empty);
            Assert.That(searchResult.User.Results, Is.Empty);
        }
    }
}
