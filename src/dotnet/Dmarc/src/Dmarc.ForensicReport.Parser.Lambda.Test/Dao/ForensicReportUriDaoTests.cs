using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReportUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicReportUriDaoTests : DatabaseTestBase
    {
        private ForensicReportUriDao _forensicReportUriDao;
        private ForensicUriDao _forensicUriDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _forensicUriDao = new ForensicUriDao();
            _forensicReportUriDao = new ForensicReportUriDao(_forensicUriDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddForensicReportUriDaoWhenDoesntExistCorrectlyAdded()
        {
            long reportId = GetReportId();
            ForensicReportUriEntity forensicReportUri = new ForensicReportUriEntity(new ForensicUriEntity("http://domain.com", "a1b2c3")) {ReportId = reportId};
            List<ForensicReportUriEntity> forensicReportUriFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicReportUriFromDao = await _forensicReportUriDao.Add(new List<ForensicReportUriEntity>{ forensicReportUri }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicReportUriFromDao.Count, Is.EqualTo(1));
            Assert.That(forensicReportUri.ReportId, Is.EqualTo(forensicReportUriFromDao[0].ReportId));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_reported_uri"))
            {
                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetInt64("report_id"), Is.EqualTo(forensicReportUriFromDao[0].ReportId));
                    Assert.That(reader.GetInt64("uri_id"), Is.EqualTo(forensicReportUriFromDao[0].ForensicUri.Id));
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThrowWhenAddedTwice()
        {
            long reportId = GetReportId();
            ForensicReportUriEntity forensicReportUri = new ForensicReportUriEntity(new ForensicUriEntity("http://domain.com", "a1b2c3")) { ReportId = reportId };
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicReportUriDao.Add(new List<ForensicReportUriEntity> { forensicReportUri }, connection, transaction);
                    Assert.ThrowsAsync<MySqlException>(async () => await _forensicReportUriDao.Add(new List<ForensicReportUriEntity> { forensicReportUri }, connection, transaction));

                    transaction.Commit();
                }
                connection.Close();
            }
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
