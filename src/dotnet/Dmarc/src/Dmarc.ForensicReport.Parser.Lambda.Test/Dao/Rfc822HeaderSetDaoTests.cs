using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822Header;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderField;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderSet;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderTextValue;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class Rfc822HeaderSetDaoTests : DatabaseTestBase
    {
        private Rfc822HeaderSetDao _rfc822HeaderSetDao;
        private ContentTypeDao _contentTypeDao;
        private Rfc822HeaderDao _rfc822HeaderDao;
        private Rfc822HeaderFieldDao _rfc822HeaderFieldDao;
        private EmailAddressDao _emailAddressDao;
        private Rfc822HeaderTextValueDao _rfc822HeaderTextValueDao;
        private IpAddressDao _ipAddressDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _contentTypeDao = new ContentTypeDao();
            _rfc822HeaderFieldDao = new Rfc822HeaderFieldDao();
            _emailAddressDao = new EmailAddressDao();
            _rfc822HeaderTextValueDao = new Rfc822HeaderTextValueDao();
            _ipAddressDao = new IpAddressDao();
            _rfc822HeaderDao = new Rfc822HeaderDao(_rfc822HeaderFieldDao, _emailAddressDao, _rfc822HeaderTextValueDao, _ipAddressDao);
            _rfc822HeaderSetDao = new Rfc822HeaderSetDao(_contentTypeDao,_rfc822HeaderDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddRfc822HeaderSetCorrectlyAdded()
        {
            long reportId = GetReportId();

            Rfc822HeaderSetEntity rfc822HeaderSet = new Rfc822HeaderSetEntity(new ContentTypeEntity("message/rfc822"), 0, 0, new List<Rfc822HeaderEntity>()) {ReportId = reportId};
            List<Rfc822HeaderSetEntity> rfc822HeaderSetFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    rfc822HeaderSetFromDao = await _rfc822HeaderSetDao.Add(new List<Rfc822HeaderSetEntity> { rfc822HeaderSet }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(rfc822HeaderSetFromDao.Count, Is.EqualTo(1));
            Assert.That(rfc822HeaderSetFromDao[0].Order, Is.EqualTo(rfc822HeaderSet.Order));
            Assert.That(rfc822HeaderSetFromDao[0].Depth, Is.EqualTo(rfc822HeaderSet.Depth));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_set"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("id"), Is.EqualTo(rfc822HeaderSet.Id));
                    Assert.That(reader.GetInt64("report_id"), Is.EqualTo(rfc822HeaderSet.ReportId));
                    Assert.That(reader.GetInt32("order"), Is.EqualTo(rfc822HeaderSet.Order));
                    Assert.That(reader.GetInt32("depth"), Is.EqualTo(rfc822HeaderSet.Depth));
                    Assert.That(reader.GetInt32("content_type_id"), Is.EqualTo(rfc822HeaderSet.ContentType.Id));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddMultipleRfc822HeaderSetsCorrectlyAdded()
        {
            long reportId = GetReportId();

            Rfc822HeaderSetEntity rfc822HeaderSet1 = new Rfc822HeaderSetEntity(new ContentTypeEntity("message/rfc822"), 0, 0, new List<Rfc822HeaderEntity>()) { ReportId = reportId };
            Rfc822HeaderSetEntity rfc822HeaderSet2 = new Rfc822HeaderSetEntity(new ContentTypeEntity("message/rfc822"), 1, 2, new List<Rfc822HeaderEntity>()) { ReportId = reportId };
            List<Rfc822HeaderSetEntity> rfc822HeaderSetFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    rfc822HeaderSetFromDao = await _rfc822HeaderSetDao.Add(new List<Rfc822HeaderSetEntity> { rfc822HeaderSet1, rfc822HeaderSet2 }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(rfc822HeaderSetFromDao.Count, Is.EqualTo(2));
            Assert.That(rfc822HeaderSetFromDao[0].Order, Is.EqualTo(rfc822HeaderSet1.Order));
            Assert.That(rfc822HeaderSetFromDao[0].Depth, Is.EqualTo(rfc822HeaderSet1.Depth));
            Assert.That(rfc822HeaderSetFromDao[1].Order, Is.EqualTo(rfc822HeaderSet2.Order));
            Assert.That(rfc822HeaderSetFromDao[1].Depth, Is.EqualTo(rfc822HeaderSet2.Depth));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM rfc822_header_set"))
            {
                while (reader.Read())
                {
                    Assert.That(reader.GetInt64("id"), Is.EqualTo(rfc822HeaderSetFromDao[count].Id));
                    Assert.That(reader.GetInt64("report_id"), Is.EqualTo(rfc822HeaderSetFromDao[count].ReportId));
                    Assert.That(reader.GetInt32("order"), Is.EqualTo(rfc822HeaderSetFromDao[count].Order));
                    Assert.That(reader.GetInt32("depth"), Is.EqualTo(rfc822HeaderSetFromDao[count].Depth));
                    Assert.That(reader.GetInt32("content_type_id"), Is.EqualTo(rfc822HeaderSetFromDao[count].ContentType.Id));
                    count++;
                }
            }

            Assert.That(count, Is.EqualTo(2));
        }

        private long GetReportId()
        {
            long ipAddressId = (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, "INSERT INTO `ip_address` (`address`, `binary_address`, `subnet_id`) VALUES ('127.0.0.1', '0x7F000001', NULL); SELECT LAST_INSERT_ID();");
            return (long)(ulong)MySqlHelper.ExecuteScalar(ConnectionString, $"INSERT INTO `forensic_report` (`original_uri`, `feedback_type`, `user_agent`, `version`, `auth_failure`, `original_envelope_id`, `arrival_date`, " +
                                                                            $"`reporting_mta`, `source_ip_id`, `incidents`, `delivery_result`, `provider_message_id`, `message_id`, `dkim_domain`, `dkim_identity`, `dkim_selector`, " +
                                                                            $"`dkim_canonicalized_header`, `spf_dns`, `authentication_results`, `reported_domain`, `created_date`, `request_id`, `dkim_canonicalized_body`) VALUES " +
                                                                            $"('', 'NULL', NULL, NULL, NULL, NULL, NULL, NULL, {ipAddressId}, NULL, NULL, '', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2017-01-01', '', NULL); SELECT LAST_INSERT_ID();");
        }
    }
}
