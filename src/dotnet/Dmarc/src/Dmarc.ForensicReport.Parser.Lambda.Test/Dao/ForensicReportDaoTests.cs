using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.TestSupport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinary;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReportUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicText;
using Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalMailFrom;
using Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalRcptTo;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderSet;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using FakeItEasy;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.ForensicReport.Parser.Lambda.Test.Dao
{
    [TestFixture]
    [Category("Integration")]
    public class ForensicReportParserDaoTests2 : DatabaseTestBase
    {
        private readonly DateTime _now = new DateTime(2017,03,24,17,9,1);

        private ForensicReportDao _forensicReportDao;
        private IConnectionInfoAsync _connectionInfoAsync;
        private IIpAddressDao _ipAddressDao;
        private IRfc822HeaderSetDao _rfc822HeaderSetDao;
        private IOriginalMailFromDao _originalMailFromDao;
        private IOrginalRcptToDao _originalRcptToDao;
        private IForensicBinaryDao _forensicBinaryDao;
        private IForensicTextDao _forensicTextDao;
        private IForensicReportUriDao _forensicReportUriDao;
        private ILogger _log;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            _connectionInfoAsync = A.Fake<IConnectionInfoAsync>();
            A.CallTo(() => _connectionInfoAsync.GetConnectionStringAsync()).Returns(ConnectionString);
            _ipAddressDao = new IpAddressDao();
            _rfc822HeaderSetDao = A.Fake<IRfc822HeaderSetDao>();
            _originalMailFromDao = A.Fake<IOriginalMailFromDao>();
            _originalRcptToDao = A.Fake<IOrginalRcptToDao>();
            _forensicBinaryDao = A.Fake<IForensicBinaryDao>();
            _forensicTextDao = A.Fake<IForensicTextDao>();
            _forensicReportUriDao = A.Fake<IForensicReportUriDao>();
            _log = A.Fake<ILogger>();

            _forensicReportDao = new ForensicReportDao(_connectionInfoAsync,
                _ipAddressDao,
                _rfc822HeaderSetDao,
                _originalMailFromDao,
                _originalRcptToDao,
                _forensicBinaryDao,
                _forensicTextDao,
                _forensicReportUriDao,
                _log);
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public async Task TestAdd()
        {
            ForensicReportEntity forensicReport = Create();
            await _forensicReportDao.Add(forensicReport);

            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_report;"))
            {
                int count = 0;

                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetString("request_id"), Is.EqualTo(forensicReport.RequestId));
                    Assert.That(reader.GetString("original_uri"), Is.EqualTo(forensicReport.OrginalUri));
                    Assert.That(reader.GetDateTime("created_date"), Is.EqualTo(forensicReport.CreatedDate));
                    Assert.That(reader.GetString("user_agent"), Is.EqualTo(forensicReport.UserAgent));
                    Assert.That(reader.GetString("version"), Is.EqualTo(forensicReport.Version));
                    Assert.That(reader.GetString("auth_failure"), Is.EqualTo(forensicReport.AuthFailure.ToString()));
                    Assert.That(reader.GetString("original_envelope_id"), Is.EqualTo(forensicReport.OriginalEnvelopeId));
                    Assert.That(reader.GetDateTime("arrival_date"), Is.EqualTo(forensicReport.ArrivalDate));
                    Assert.That(reader.GetString("reporting_mta"), Is.EqualTo(forensicReport.ReportingMta));
                    Assert.That(reader.GetInt64Nullable("source_ip_id"), Is.EqualTo(forensicReport.SourceIp.Id));
                    Assert.That(reader.GetInt32Nullable("incidents"), Is.EqualTo(forensicReport.Incidents));
                    Assert.That(reader.GetString("delivery_result"), Is.EqualTo(forensicReport.DeliveryResult.ToString()));
                    Assert.That(reader.GetString("message_id"), Is.EqualTo(forensicReport.MessageId));
                    Assert.That(reader.GetString("provider_message_id"), Is.EqualTo(forensicReport.ProviderMessageId));
                    Assert.That(reader.GetString("dkim_domain"), Is.EqualTo(forensicReport.DkimDomain));
                    Assert.That(reader.GetString("dkim_identity"), Is.EqualTo(forensicReport.DkimIdentity));
                    Assert.That(reader.GetString("dkim_selector"), Is.EqualTo(forensicReport.DkimSelector));
                    Assert.That(reader.GetString("dkim_canonicalized_header"), Is.EqualTo(forensicReport.DkimCanonicalizedHeader));
                    Assert.That(reader.GetString("dkim_canonicalized_body"), Is.EqualTo(forensicReport.DkimCanonicalizedBody));
                    Assert.That(reader.GetString("spf_dns"), Is.EqualTo(forensicReport.SpfDns));
                    Assert.That(reader.GetString("authentication_results"), Is.EqualTo(forensicReport.AuthenticationResults));
                    Assert.That(reader.GetString("reported_domain"), Is.EqualTo(forensicReport.ReportedDomain));
                }

                Assert.That(count, Is.EqualTo(1));
                A.CallTo(() => _rfc822HeaderSetDao.Add(A<List<Rfc822HeaderSetEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _originalMailFromDao.Add(A<List<EmailAddressReportEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _originalRcptToDao.Add(A<List<EmailAddressReportEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _forensicBinaryDao.Add(A<List<ForensicBinaryEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _forensicTextDao.Add(A<List<ForensicTextEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _forensicReportUriDao.Add(A<List<ForensicReportUriEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Test]
        public async Task DuplicateReportNotAdded()
        {
            ForensicReportEntity forensicReport = Create();
            await _forensicReportDao.Add(forensicReport);
            await _forensicReportDao.Add(forensicReport);

            using (DbDataReader reader = MySqlHelper.ExecuteReader(ConnectionString, "SELECT * FROM forensic_report;"))
            {
                int count = 0;

                while (reader.Read())
                {
                    count++;
                    Assert.That(reader.GetString("request_id"), Is.EqualTo(forensicReport.RequestId));
                    Assert.That(reader.GetString("original_uri"), Is.EqualTo(forensicReport.OrginalUri));
                    Assert.That(reader.GetDateTime("created_date"), Is.EqualTo(forensicReport.CreatedDate));
                    Assert.That(reader.GetString("user_agent"), Is.EqualTo(forensicReport.UserAgent));
                    Assert.That(reader.GetString("version"), Is.EqualTo(forensicReport.Version));
                    Assert.That(reader.GetString("auth_failure"), Is.EqualTo(forensicReport.AuthFailure.ToString()));
                    Assert.That(reader.GetString("original_envelope_id"), Is.EqualTo(forensicReport.OriginalEnvelopeId));
                    Assert.That(reader.GetDateTime("arrival_date"), Is.EqualTo(forensicReport.ArrivalDate));
                    Assert.That(reader.GetString("reporting_mta"), Is.EqualTo(forensicReport.ReportingMta));
                    Assert.That(reader.GetInt64Nullable("source_ip_id"), Is.EqualTo(forensicReport.SourceIp.Id));
                    Assert.That(reader.GetInt32Nullable("incidents"), Is.EqualTo(forensicReport.Incidents));
                    Assert.That(reader.GetString("delivery_result"), Is.EqualTo(forensicReport.DeliveryResult.ToString()));
                    Assert.That(reader.GetString("message_id"), Is.EqualTo(forensicReport.MessageId));
                    Assert.That(reader.GetString("provider_message_id"), Is.EqualTo(forensicReport.ProviderMessageId));
                    Assert.That(reader.GetString("dkim_domain"), Is.EqualTo(forensicReport.DkimDomain));
                    Assert.That(reader.GetString("dkim_identity"), Is.EqualTo(forensicReport.DkimIdentity));
                    Assert.That(reader.GetString("dkim_selector"), Is.EqualTo(forensicReport.DkimSelector));
                    Assert.That(reader.GetString("dkim_canonicalized_header"), Is.EqualTo(forensicReport.DkimCanonicalizedHeader));
                    Assert.That(reader.GetString("dkim_canonicalized_body"), Is.EqualTo(forensicReport.DkimCanonicalizedBody));
                    Assert.That(reader.GetString("spf_dns"), Is.EqualTo(forensicReport.SpfDns));
                    Assert.That(reader.GetString("authentication_results"), Is.EqualTo(forensicReport.AuthenticationResults));
                    Assert.That(reader.GetString("reported_domain"), Is.EqualTo(forensicReport.ReportedDomain));
                }

                Assert.That(count, Is.EqualTo(1));
                A.CallTo(() => _rfc822HeaderSetDao.Add(A<List<Rfc822HeaderSetEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _originalMailFromDao.Add(A<List<EmailAddressReportEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _originalRcptToDao.Add(A<List<EmailAddressReportEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _forensicBinaryDao.Add(A<List<ForensicBinaryEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _forensicTextDao.Add(A<List<ForensicTextEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _forensicReportUriDao.Add(A<List<ForensicReportUriEntity>>._, A<MySqlConnection>._, A<MySqlTransaction>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _log.Error(A<string>.That.Matches(_ => _.StartsWith("Duplicate")))).MustHaveHappened(Repeated.Exactly.Once);
            }
        }



        private ForensicReportEntity Create()
        {
            return new ForensicReportEntity
            {
                RequestId = "0123456789abcdef",
                OrginalUri = "bucket/abcdef0123456789",
                CreatedDate = _now,
                FeedbackType = FeedbackType.AuthFailure,
                UserAgent = "Lua/1.0",
                Version = "1.0",
                AuthFailure = AuthFailure.Dmarc,
                OriginalEnvelopeId = "abcdef0123456789",
                OriginalMailFroms = new List<EmailAddressReportEntity> { new EmailAddressReportEntity(new EmailAddressEntity(""))},
                ArrivalDate = _now,
                ReportingMta = "dns;gov.uk",
                SourceIp = new IpAddressEntity("127.0.0.1", ""),
                Incidents = null,
                DeliveryResult = DeliveryResult.Delivered,
                MessageId = "abcdef0123456789@gov.uk",
                ProviderMessageId = "abcdef0123456789@abc.uk",
                DkimDomain = "gov.uk",
                DkimIdentity = "@gov.uk",
                DkimSelector = "abcd",
                DkimCanonicalizedHeader = "abcdef0123456789",
                DkimCanonicalizedBody = "abcdef0123456789",
                SpfDns = "txt:gov.uk",
                AuthenticationResults = "dmarc=fail",
                ReportedDomain = "gov.uk",
                OrginalRcptTos = new List<EmailAddressReportEntity> { new EmailAddressReportEntity(new EmailAddressEntity(""))},
                Rfc822HeaderSets = new List<Rfc822HeaderSetEntity> { new Rfc822HeaderSetEntity(new ContentTypeEntity(""), 0, 0, new List<Rfc822HeaderEntity>())},
                BinaryMessageParts = new List<ForensicBinaryEntity> { new ForensicBinaryEntity(new ForensicBinaryContentEntity(new byte[0], new List<HashEntity>()), new ContentTypeEntity(""), "", "", ContentDisposition.Attachment, 0, 0)},
                TextMessageParts = new List<ForensicTextEntity> { new ForensicTextEntity(new ForensicTextContentEntity("", new List<HashEntity>(), new List<ForensicTextContentUriEntity>()), new ContentTypeEntity(""), 0, 0 )},
                ReportedUris = new List<ForensicReportUriEntity> { new ForensicReportUriEntity(new ForensicUriEntity("",""))}
            };
        }
    }
}
