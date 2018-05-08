using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicText;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContent;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContentUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicTextDaoTests : DatabaseTestBase
    {
        private ForensicTextDao _forensicTextDao;
        private ForensicTextContentDao _forensicTextContentDao;
        private ContentTypeDao _contentTypeDao;
        private ForensicTextHashDao _forensicTextHashDao;
        private ForensicTextContentUriDao _forensicTextContentUriDao;
        private ForensicUriDao _forensicUriDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _forensicTextHashDao = new ForensicTextHashDao();
            _forensicUriDao = new ForensicUriDao();
            _forensicTextContentUriDao = new ForensicTextContentUriDao(_forensicUriDao);
            _forensicTextContentDao = new ForensicTextContentDao(_forensicTextHashDao, _forensicTextContentUriDao);
            _contentTypeDao = new ContentTypeDao();
            _forensicTextDao = new ForensicTextDao(_forensicTextContentDao, _contentTypeDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddForensicTextWhenDoesntExistCorrectlyAdded()
        {
            long reportId = GetReportId();

            ForensicTextEntity forensicTextEntity = Create(reportId, "A5GHJ89==");
            List<ForensicTextEntity> forensicTextEntitiesFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicTextEntitiesFromDao = await _forensicTextDao.Add(new List<ForensicTextEntity> { forensicTextEntity }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicTextEntitiesFromDao.Count, Is.EqualTo(1));
            Assert.That(forensicTextEntitiesFromDao[0].Depth, Is.EqualTo(forensicTextEntity.Depth));
            Assert.That(forensicTextEntitiesFromDao[0].Order, Is.EqualTo(forensicTextEntity.Order));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_text_match"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetInt64("report_id"), Is.EqualTo(forensicTextEntitiesFromDao[0].ReportId));
                    Assert.That(reader.GetInt64("body_id"), Is.EqualTo(forensicTextEntitiesFromDao[0].ForensicTextContent.Id));
                    Assert.That(reader.GetInt64("content_type_id"), Is.EqualTo(forensicTextEntitiesFromDao[0].ContentType.Id));
                    Assert.That(reader.GetInt16("order"), Is.EqualTo(forensicTextEntitiesFromDao[0].Order));
                    Assert.That(reader.GetInt16("depth"), Is.EqualTo(forensicTextEntitiesFromDao[0].Depth));

                }
            }
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddMultipleForensicTextCorrectlyAdded()

        {
            long reportId = GetReportId();

            ForensicTextEntity forensicTextEntity1 = Create(reportId, "A5GHJ89==");
            ForensicTextEntity forensicTextEntity2 = Create(reportId, "B5GHJ89==");
            List<ForensicTextEntity> forensicTextEntitiesFromDao;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicTextEntitiesFromDao = await _forensicTextDao.Add(new List<ForensicTextEntity> { forensicTextEntity1, forensicTextEntity2 }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicTextEntitiesFromDao.Count, Is.EqualTo(2));
            Assert.That(forensicTextEntitiesFromDao[0].Depth, Is.EqualTo(forensicTextEntity1.Depth));
            Assert.That(forensicTextEntitiesFromDao[0].Order, Is.EqualTo(forensicTextEntity1.Order));
            Assert.That(forensicTextEntitiesFromDao[1].Depth, Is.EqualTo(forensicTextEntity2.Depth));
            Assert.That(forensicTextEntitiesFromDao[1].Order, Is.EqualTo(forensicTextEntity2.Order));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_text_match"))
            {
                while (reader.Read())
                {
                    Assert.That(reader.GetInt64("report_id"), Is.EqualTo(forensicTextEntitiesFromDao[count].ReportId));
                    Assert.That(reader.GetInt64("body_id"), Is.EqualTo(forensicTextEntitiesFromDao[count].ForensicTextContent.Id));
                    Assert.That(reader.GetInt64("content_type_id"), Is.EqualTo(forensicTextEntitiesFromDao[count].ContentType.Id));
                    Assert.That(reader.GetInt16("order"), Is.EqualTo(forensicTextEntitiesFromDao[count].Order));
                    Assert.That(reader.GetInt16("depth"), Is.EqualTo(forensicTextEntitiesFromDao[count].Depth));
                    count++;

                }
            }
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task ThrowsIfInsertedTwice()
        {
            long reportId = GetReportId();

            ForensicTextEntity forensicTextEntity = Create(reportId, "A5GHJ89==");
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicTextDao.Add(new List<ForensicTextEntity> { forensicTextEntity }, connection, transaction);
                    Assert.ThrowsAsync<MySqlException>(async () => await _forensicTextDao.Add(new List<ForensicTextEntity> { forensicTextEntity }, connection, transaction));

                    transaction.Commit();
                }
                connection.Close();
            }
        }

        private ForensicTextEntity Create(long reportId, string hash)
        {
            ForensicTextContentEntity forensicTextContentEntity = new ForensicTextContentEntity("", new List<HashEntity> {new HashEntity(EntityHashType.Sha1, hash)}, new List<ForensicTextContentUriEntity>() );
            ContentTypeEntity contentTypeEntity = new ContentTypeEntity("text/html");
            return new ForensicTextEntity(forensicTextContentEntity, contentTypeEntity, 1, 1) {ReportId = reportId};
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
