using System.Collections.Generic;
using System.IO;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Single;
using Microsoft.Data.Sqlite;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.FileSystem
{
    public class SqliteDenormalisedRecordPersistor : IDenormalisedRecordPersistor
    {
        private const string CreateTable = "CREATE TABLE IF NOT EXISTS rua (orginal_url text, " +
            "org_name text, email text, extra_contact_info text, date_range_begin text, date_range_end text, " +
            "domain text, adkim text, aspf text, p text, sp text, pct text, source_ip text, count text, " +
            "disposition text, dkim text, spf text, reason_type text, comment text, envelope_to " +
            "text, header_from text, dkim_domain text, dkim_result text, dkim_hresult text, " +
            "spf_domain text, spf_result text);";

        private const string AddRecord = "INSERT INTO rua (orginal_url, org_name, email, " +
            "extra_contact_info, date_range_begin, date_range_end, domain, adkim, aspf, " +
            "p, sp, pct, source_ip, count, disposition, dkim, spf, reason_type, comment, envelope_to," +
            " header_from, dkim_domain, dkim_result, dkim_hresult, spf_domain, spf_result) " +
            "values($OriginalUri,$OrgName,$Email,$ExtraContactInfo,$BeginDate,$EndDate,$Domain,$Adkim," +
            "$Aspf,$P,$Sp,$Pct,$SourceIp,$Count,$Disposition,$Dkim,$Spf,$Reason,$Comment,$EnvelopeTo" +
            ",$HeaderFrom,$DkimDomain,$DkimResult,$DkimHumanResult,$SpfDomain,$SpfResult);";

        private readonly FileInfo _location;
        private bool _inited;

        public SqliteDenormalisedRecordPersistor(FileInfo location)
        {
            _location = location;
        }

        public void Persist(IEnumerable<DenormalisedRecord> denormalisedRecords)
        {
            CreateDirectoryAndRemoveOldFiles();

            using (var connection = new SqliteConnection($"DataSource={_location.FullName};Cache=Shared"))
            {
                connection.Open();

                ConfigureDatabase(connection);

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (DenormalisedRecord denormalisedRecord in denormalisedRecords)
                    {
                        var command = connection.CreateCommand();

                        command.Transaction = transaction;
                        command.CommandText = AddRecord;
                        
                        CreateParameters(command, denormalisedRecord);

                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }

                connection.Close();
            }
        }

        private static void CreateParameters(SqliteCommand command, DenormalisedRecord denormalisedRecord)
        {
            command.Parameters.AddWithValue("$OriginalUri", denormalisedRecord.OrginalUri ?? string.Empty);
            command.Parameters.AddWithValue("$OrgName", denormalisedRecord.OrgName ?? string.Empty);
            command.Parameters.AddWithValue("$Email", denormalisedRecord.Email ?? string.Empty);
            command.Parameters.AddWithValue("$ExtraContactInfo", denormalisedRecord.ExtraContactInfo ?? string.Empty);
            command.Parameters.AddWithValue("$BeginDate", denormalisedRecord.BeginDate.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$EndDate", denormalisedRecord.EndDate.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Domain", denormalisedRecord.Domain ?? string.Empty);
            command.Parameters.AddWithValue("$Adkim", denormalisedRecord.Adkim?.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Aspf", denormalisedRecord.Aspf?.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$P", denormalisedRecord.P.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Sp", denormalisedRecord.Sp?.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Pct", denormalisedRecord.Pct?.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$SourceIp", denormalisedRecord.SourceIp ?? string.Empty);
            command.Parameters.AddWithValue("$Count", denormalisedRecord.Count.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Disposition", denormalisedRecord.Disposition?.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Dkim", denormalisedRecord.Dkim?.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Spf", denormalisedRecord.Spf?.ToString() ?? string.Empty);
            command.Parameters.AddWithValue("$Reason", denormalisedRecord.Reason ?? string.Empty);
            command.Parameters.AddWithValue("$Comment", denormalisedRecord.Comment ?? string.Empty);
            command.Parameters.AddWithValue("$EnvelopeTo", denormalisedRecord.EnvelopeTo ?? string.Empty);
            command.Parameters.AddWithValue("$HeaderFrom", denormalisedRecord.HeaderFrom ?? string.Empty);
            command.Parameters.AddWithValue("$DkimDomain", denormalisedRecord.DkimDomain ?? string.Empty);
            command.Parameters.AddWithValue("$DkimResult", denormalisedRecord.DkimResult ?? string.Empty);
            command.Parameters.AddWithValue("$DkimHumanResult", denormalisedRecord.DkimHumanResult ?? string.Empty);
            command.Parameters.AddWithValue("$SpfDomain", denormalisedRecord.SpfDomain ?? string.Empty);
            command.Parameters.AddWithValue("$SpfResult", denormalisedRecord.SpfResult ?? string.Empty);
        }

        private void ConfigureDatabase(SqliteConnection connection)
        {
            ExecuteNonQuery(connection, "PRAGMA foreign_keys=OFF");
            ExecuteNonQuery(connection, "PRAGMA synchronous=OFF");
            ExecuteNonQuery(connection, "PRAGMA count_changes=OFF");
            ExecuteNonQuery(connection, "PRAGMA temp_store=MEMORY");
            ExecuteNonQuery(connection, "PRAGMA journal_mode = MEMORY");
            ExecuteNonQuery(connection, "PRAGMA cache_size=10000");
            ExecuteNonQuery(connection, CreateTable);
        }

        private void ExecuteNonQuery(SqliteConnection connection, string commandText)
        {
            SqliteCommand command1 = connection.CreateCommand();
            command1.CommandText = commandText;
            command1.ExecuteNonQuery();
        }

        private void CreateDirectoryAndRemoveOldFiles()
        {
            if (!_inited)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(_location.DirectoryName);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                if (_location.Exists)
                {
                    _location.Delete();
                }
                _inited = true;
            }
        }
    }
}
