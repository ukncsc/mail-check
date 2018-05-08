using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalMailFrom;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class OriginalMailFromDaoTests : DatabaseTestBase
    {
        private EmailAddressDao _emailAddressDao;
        private OriginalMailFromDao _originalMailFromDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _emailAddressDao = new EmailAddressDao();
            _originalMailFromDao = new OriginalMailFromDao(_emailAddressDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddOriginalMailFromWhenDoesntExistCorrectlyAdded()
        {
            long reportId = GetReportId();

            EmailAddressReportEntity emailAddressReport = new EmailAddressReportEntity(new EmailAddressEntity("test@gov.uk")) {ReportId = reportId};
            List<EmailAddressReportEntity> emailAddressReportsFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    emailAddressReportsFromDao = await _originalMailFromDao.Add(new List<EmailAddressReportEntity> { emailAddressReport }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(emailAddressReportsFromDao.Count, Is.EqualTo(1));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_report_mail_from"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetInt64("report_id"), Is.EqualTo(emailAddressReportsFromDao[0].ReportId));
                    Assert.That(reader.GetInt64("original_mail_from_id"), Is.EqualTo(emailAddressReportsFromDao[0].EmailAddressEntity.Id));

                }
            }
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddTwiceThrowsException()
        {
            long reportId = GetReportId();

            EmailAddressReportEntity emailAddressReport = new EmailAddressReportEntity(new EmailAddressEntity("test@gov.uk")) {ReportId = reportId};

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _originalMailFromDao.Add(new List<EmailAddressReportEntity> {emailAddressReport}, connection, transaction);
                    Assert.ThrowsAsync<MySqlException>(async () => await _originalMailFromDao.Add(new List<EmailAddressReportEntity> { emailAddressReport }, connection, transaction));
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