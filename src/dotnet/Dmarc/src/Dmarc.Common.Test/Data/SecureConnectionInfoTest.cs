using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using FakeItEasy;
using NUnit.Framework;

namespace Dmarc.Common.Test.Data
{
    [TestFixture]
    public class SecureConnectionInfoTest
    {
        private readonly string _connectionString =
            "Server = dev-cluster.cluster-cn7nidgb1mh9.eu-west-1.rds.amazonaws.com; Port = 3306; Database = dmarc; Uid = dev_aggproc; Connection Timeout=5;";

        private readonly string _connectionStringWithPassword =
            "Server = dev-cluster.cluster-cn7nidgb1mh9.eu-west-1.rds.amazonaws.com; Port = 3306; Database = dmarc; Uid = dev_aggproc; Connection Timeout=5;Pwd = TopSecretPassword;";

        private IParameterStoreRequest _parameterStoreRequest;

        private IConnectionInfoAsync _connectionInfoAsync;


        [SetUp]
        public void SetUp()
        {
            _parameterStoreRequest = A.Fake<IParameterStoreRequest>();
            IConnectionInfo connectionInfo = new StringConnectionInfo(_connectionString);
            _connectionInfoAsync = new ConnectionInfoAsync(_parameterStoreRequest, connectionInfo);
        }

        [Test]
        public async Task GetSecureConnectionStringTest()
        {
            string paramvalue = "TopSecretPassword";
            A.CallTo(() => _parameterStoreRequest.GetParameterValue(A<string>._)).Returns(Task.FromResult(paramvalue));

            var connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            Assert.That(connectionString, Is.EqualTo(_connectionStringWithPassword));
        }

        [Test]
        public async Task GetSecureConnectionStringNonNullPwd()
        {
            string paramvalue = "TopSecretPassword";
            IConnectionInfo connectionInfo = new StringConnectionInfo(_connectionStringWithPassword);
            IConnectionInfoAsync connectionInfoAsyncWithPassword = new ConnectionInfoAsync(_parameterStoreRequest,connectionInfo);
            A.CallTo(() => _parameterStoreRequest.GetParameterValue(A<string>._)).Returns(Task.FromResult(paramvalue));

            var connectionString = await connectionInfoAsyncWithPassword.GetConnectionStringAsync();

            Assert.That(connectionString,Is.EqualTo(_connectionStringWithPassword));

        }


    }
}
