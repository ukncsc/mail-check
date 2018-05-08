using System.Diagnostics;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Persistance.Dao;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinary;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReportUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicText;
using Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalMailFrom;
using Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalRcptTo;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderSet;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReport
{
    public class ForensicReportDao : IReportDaoAsync<ForensicReportEntity>
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly IIpAddressDao _ipAddressDao;
        private readonly IRfc822HeaderSetDao _rfc822HeaderSetDao;
        private readonly IOriginalMailFromDao _originalMailFromDao;
        private readonly IOrginalRcptToDao _orginalRcptToDao;
        private readonly IForensicBinaryDao _forensicBinaryDao;
        private readonly IForensicTextDao _forensicTextDao;
        private readonly IForensicReportUriDao _forensicReportUriDao;
        private readonly ILogger _log;

        public ForensicReportDao(
            IConnectionInfoAsync connectionInfo, 
            IIpAddressDao ipAddressDao,
            IRfc822HeaderSetDao rfc822HeaderSetDao,
            IOriginalMailFromDao originalMailFromDao,
            IOrginalRcptToDao orginalRcptToDao,
            IForensicBinaryDao forensicBinaryDao,
            IForensicTextDao forensicTextDao,
            IForensicReportUriDao forensicReportUriDao,
            ILogger log)
        {
            _connectionInfo = connectionInfo;
            _ipAddressDao = ipAddressDao;
            _rfc822HeaderSetDao = rfc822HeaderSetDao;
            _originalMailFromDao = originalMailFromDao;
            _orginalRcptToDao = orginalRcptToDao;
            _forensicBinaryDao = forensicBinaryDao;
            _forensicTextDao = forensicTextDao;
            _forensicReportUriDao = forensicReportUriDao;
            _log = log;
        }

        public async Task<bool> Add(ForensicReportEntity forensicReportEntity)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            bool added = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    IpAddressEntity sourceIp = await _ipAddressDao.Add(forensicReportEntity.SourceIp, connection, transaction);
                    forensicReportEntity.SourceIp = sourceIp;

                    _log.Debug(forensicReportEntity.ToString());

                    MySqlCommand command = new MySqlCommand(ForensicReportDaoResources.InsertForensicReport, connection, transaction);
                    command.Parameters.AddWithValue("original_uri", forensicReportEntity.OrginalUri);
                    command.Parameters.AddWithValue("feedback_type", forensicReportEntity.FeedbackType?.ToString());
                    command.Parameters.AddWithValue("user_agent", forensicReportEntity.UserAgent);
                    command.Parameters.AddWithValue("version", forensicReportEntity.Version);
                    command.Parameters.AddWithValue("auth_failure", forensicReportEntity.AuthFailure?.ToString());
                    command.Parameters.AddWithValue("original_envelope_id", forensicReportEntity.OriginalEnvelopeId);
                    command.Parameters.AddWithValue("arrival_date", forensicReportEntity.ArrivalDate);
                    command.Parameters.AddWithValue("reporting_mta", forensicReportEntity.ReportingMta);
                    command.Parameters.AddWithValue("source_ip_id", forensicReportEntity.SourceIp.Id);
                    command.Parameters.AddWithValue("incidents", forensicReportEntity.Incidents);
                    command.Parameters.AddWithValue("delivery_result", forensicReportEntity.DeliveryResult?.ToString());
                    command.Parameters.AddWithValue("message_id", forensicReportEntity.MessageId);
                    command.Parameters.AddWithValue("provider_message_id", forensicReportEntity.ProviderMessageId);
                    command.Parameters.AddWithValue("dkim_domain", forensicReportEntity.DkimDomain);
                    command.Parameters.AddWithValue("dkim_identity", forensicReportEntity.DkimIdentity);
                    command.Parameters.AddWithValue("dkim_selector", forensicReportEntity.DkimSelector);
                    command.Parameters.AddWithValue("dkim_canonicalized_header", forensicReportEntity.DkimCanonicalizedHeader);
                    command.Parameters.AddWithValue("dkim_canonicalized_body", forensicReportEntity.DkimCanonicalizedBody);
                    command.Parameters.AddWithValue("spf_dns", forensicReportEntity.SpfDns);
                    command.Parameters.AddWithValue("authentication_results", forensicReportEntity.AuthenticationResults);
                    command.Parameters.AddWithValue("reported_domain", forensicReportEntity.ReportedDomain);
                    command.Parameters.AddWithValue("created_date", forensicReportEntity.CreatedDate);
                    command.Parameters.AddWithValue("request_id", forensicReportEntity.RequestId);

                    int numberOfUpdates = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    
                    if (numberOfUpdates != 0) // number of updated == 0 -> duplicate report id.             
                    {
                        forensicReportEntity.Id = command.LastInsertedId;

                        forensicReportEntity.Rfc822HeaderSets.ForEach(_ => _.ReportId = forensicReportEntity.Id);
                        forensicReportEntity.OriginalMailFroms.ForEach(_ => _.ReportId = forensicReportEntity.Id);
                        forensicReportEntity.OrginalRcptTos.ForEach(_ => _.ReportId = forensicReportEntity.Id);
                        forensicReportEntity.BinaryMessageParts.ForEach(_ => _.ReportId = forensicReportEntity.Id);
                        forensicReportEntity.TextMessageParts.ForEach(_ => _.ReportId = forensicReportEntity.Id);
                        forensicReportEntity.ReportedUris.ForEach(_ => _.ReportId = forensicReportEntity.Id);
                      
                        forensicReportEntity.Rfc822HeaderSets = await _rfc822HeaderSetDao.Add(forensicReportEntity.Rfc822HeaderSets, connection, transaction);
                        forensicReportEntity.OriginalMailFroms = await _originalMailFromDao.Add(forensicReportEntity.OriginalMailFroms, connection, transaction);
                        forensicReportEntity.OrginalRcptTos = await _orginalRcptToDao.Add(forensicReportEntity.OrginalRcptTos, connection, transaction);
                        forensicReportEntity.BinaryMessageParts = await _forensicBinaryDao.Add(forensicReportEntity.BinaryMessageParts, connection, transaction);
                        forensicReportEntity.TextMessageParts = await _forensicTextDao.Add(forensicReportEntity.TextMessageParts, connection, transaction);
                        forensicReportEntity.ReportedUris = await _forensicReportUriDao.Add(forensicReportEntity.ReportedUris, connection, transaction);

                        added = true;
                    }
                    else
                    {
                        _log.Error($"Duplicate forensic report, request id: {forensicReportEntity.RequestId}, original uri: {forensicReportEntity.OrginalUri}, provider message id: {forensicReportEntity.ProviderMessageId}");
                    }

                    await transaction.CommitAsync().ConfigureAwait(false); ;
                    connection.Close();
                }
            }
            _log.Debug($"Persisting forensic report took {stopwatch.Elapsed}");
            stopwatch.Stop();
            return added;
        }
    }
}
