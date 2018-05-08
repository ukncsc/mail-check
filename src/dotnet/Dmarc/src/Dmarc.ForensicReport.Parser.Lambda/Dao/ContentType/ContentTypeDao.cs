using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using MySql.Data.MySqlClient;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType
{
    public interface IContentTypeDao
    {
        Task<ContentTypeEntity> Add(ContentTypeEntity contentTypeEntity, MySqlConnection connection, MySqlTransaction transaction);

        //Task<List<ContentTypeEntity>> Add(List<ContentTypeEntity> contentTypes, MySqlConnection connection, MySqlTransaction transaction);
    }

    public class ContentTypeDao : IContentTypeDao
    {
        public async Task<ContentTypeEntity> Add(ContentTypeEntity contentTypeEntity, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(ContentTypeDaoResources.InsertContentType, connection, transaction);
            command.Parameters.AddWithValue("name", contentTypeEntity.Name);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            contentTypeEntity.Id = (int)command.LastInsertedId;

            return contentTypeEntity;
        }

//        public async Task<List<ContentTypeEntity>> Add(List<ContentTypeEntity> contentTypes, MySqlConnection connection, MySqlTransaction transaction)
//        {
//            string parameterisedInsert = string.Join(",", contentTypes.Select((value, index) => $"(@a{index})"));
//            string parameterisedQuery = string.Join(",", contentTypes.Select((value, index) => $"@a{index}"));
//
//            string query = string.Format(ContentTypeDaoResources.InsertContentTypeMulti, parameterisedInsert, parameterisedQuery);
//
//            MySqlCommand command = new MySqlCommand(query, connection, transaction);
//
//            contentTypes.ForEach((value, index) => command.Parameters.Add($"a{index}", value.Name));
//
//            List<ContentTypeEntity> contentTypesToReturn = new List<ContentTypeEntity>();
//            using (DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
//            {
//                while (await reader.ReadAsync())
//                {
//                    contentTypesToReturn.Add(new ContentTypeEntity(reader.GetString("name")) { Id = reader.GetInt32("id")});
//                }
//            }
//            return contentTypesToReturn;
//        }
    }
}
