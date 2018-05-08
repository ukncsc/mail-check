using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinary;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryContent;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Org.BouncyCastle.Crypto.Tls;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicBinaryDaoTests : DatabaseTestBase
    {
        private ForensicBinaryDao _forensicBinaryDao;
        private ForensicBinaryContentDao _forensicBinaryContentDao;
        private ContentTypeDao _contentTypeDao;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _forensicBinaryContentDao = new ForensicBinaryContentDao(new ForensicBinaryHashDao());
            _contentTypeDao = new ContentTypeDao();
            _forensicBinaryDao = new ForensicBinaryDao(_forensicBinaryContentDao, _contentTypeDao);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task AddForensicBinaryWhenDoesntExistCorrectlyAdded()
        {
            long reportId = GetReportId();

            ForensicBinaryEntity forensicBinaryEntity = Create(reportId);
            List<ForensicBinaryEntity> forensicBinaryEntitiesFromDao;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    forensicBinaryEntitiesFromDao = await _forensicBinaryDao.Add(new List<ForensicBinaryEntity> { forensicBinaryEntity }, connection, transaction);
                    transaction.Commit();
                }
                connection.Close();
            }

            Assert.That(forensicBinaryEntitiesFromDao.Count, Is.EqualTo(1));
            Assert.That(forensicBinaryEntitiesFromDao[0].Filename, Is.EqualTo(forensicBinaryEntity.Filename));
            Assert.That(forensicBinaryEntitiesFromDao[0].Extension, Is.EqualTo(forensicBinaryEntity.Extension));
            Assert.That(forensicBinaryEntitiesFromDao[0].Disposition, Is.EqualTo(forensicBinaryEntity.Disposition));
            Assert.That(forensicBinaryEntitiesFromDao[0].Depth, Is.EqualTo(forensicBinaryEntity.Depth));
            Assert.That(forensicBinaryEntitiesFromDao[0].Order, Is.EqualTo(forensicBinaryEntity.Order));

            int count = 0;
            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_binary_match"))
            {
                while (reader.Read())
                {
                    count++;

                    Assert.That(reader.GetInt64("report_id"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].ReportId));
                    Assert.That(reader.GetInt64("binary_id"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].ForensicBinaryContent.Id));
                    Assert.That(reader.GetString("filename"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].Filename));
                    Assert.That(reader.GetString("extension"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].Extension));
                    Assert.That(reader.GetString("disposition"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].Disposition.GetDbName()));
                    Assert.That(reader.GetInt64("content_type_id"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].ContentType.Id));
                    Assert.That(reader.GetInt16("order"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].Order));
                    Assert.That(reader.GetInt16("depth"), Is.EqualTo(forensicBinaryEntitiesFromDao[0].Depth));
                    
                }
            }
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThrowsIfInsertedTwice()
        {
            long reportId = GetReportId();

            ForensicBinaryEntity forensicBinaryEntity = Create(reportId);
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    await _forensicBinaryDao.Add(new List<ForensicBinaryEntity> { forensicBinaryEntity }, connection, transaction);
                    Assert.ThrowsAsync<MySqlException>(async () => await _forensicBinaryDao.Add(new List<ForensicBinaryEntity> { forensicBinaryEntity }, connection, transaction));
                        
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        private ForensicBinaryEntity Create(long reportId)
        {
            ForensicBinaryContentEntity forensicBinaryContent = new ForensicBinaryContentEntity(new byte[] { 0x20, 0x20, 0x20, 0x20 }, new List<HashEntity> { new HashEntity(EntityHashType.Sha1, "A2D3F2==") });

            ContentTypeEntity contentType = new ContentTypeEntity("application/octet-stream");

            return new ForensicBinaryEntity(forensicBinaryContent, contentType, "attachment.pdf", "pdf", ContentDisposition.Attachment, 0, 0) {ReportId = reportId};
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
