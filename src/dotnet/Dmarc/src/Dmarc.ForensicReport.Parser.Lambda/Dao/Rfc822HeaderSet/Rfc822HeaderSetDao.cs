using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822Header;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderSet
{
    public interface IRfc822HeaderSetDao
    {
        Task<List<Rfc822HeaderSetEntity>> Add(List<Rfc822HeaderSetEntity> rfc822HeaderSetEntities, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class Rfc822HeaderSetDao : IRfc822HeaderSetDao
    {
        private readonly IContentTypeDao _contentTypeDao;
        private readonly IRfc822HeaderDao _headerDao;

        public Rfc822HeaderSetDao(IContentTypeDao contentTypeDao,
            IRfc822HeaderDao headerDao)
        {
            _contentTypeDao = contentTypeDao;
            _headerDao = headerDao;
        }

        public async Task<List<Rfc822HeaderSetEntity>> Add(List<Rfc822HeaderSetEntity> rfc822HeaderSetEntities, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (!rfc822HeaderSetEntities.Any())
            {
                return rfc822HeaderSetEntities;
            }

            foreach (Rfc822HeaderSetEntity rfc822HeaderSetEntity in rfc822HeaderSetEntities)
            {
                rfc822HeaderSetEntity.ContentType = await _contentTypeDao.Add(rfc822HeaderSetEntity.ContentType, connection, transaction);
            }

            MySqlCommand command = new MySqlCommand(connection, transaction);

            StringBuilder stringBuilder = new StringBuilder(Rfc822HeaderSetDaoResources.InsertRfc822HeaderSet);

            for (int i = 0; i < rfc822HeaderSetEntities.Count; i++)
            {
                stringBuilder.Append(string.Format(
                    Rfc822HeaderSetDaoResources.InsertRfc822HeaderSetValueFormatString, i));
                stringBuilder.Append(i < rfc822HeaderSetEntities.Count - 1 ? "," : ";");

                command.Parameters.AddWithValue($"a{i}", rfc822HeaderSetEntities[i].ReportId);
                command.Parameters.AddWithValue($"b{i}", rfc822HeaderSetEntities[i].Order);
                command.Parameters.AddWithValue($"c{i}", rfc822HeaderSetEntities[i].Depth);
                command.Parameters.AddWithValue($"d{i}", rfc822HeaderSetEntities[i].ContentType.Id);
            }

            command.CommandText = stringBuilder.ToString();

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            long lastInsertId = command.LastInsertedId;
            rfc822HeaderSetEntities.ForEach(_ => _.Id = lastInsertId++);

            foreach (Rfc822HeaderSetEntity rfc822HeaderSetEntity in rfc822HeaderSetEntities)
            {
                foreach (Rfc822HeaderEntity header in rfc822HeaderSetEntity.Headers)
                {
                    header.HeaderSetId = rfc822HeaderSetEntity.Id;
                    await _headerDao.Add(header, connection, transaction);
                }
            }

            return rfc822HeaderSetEntities;
        }
    }
}
