using System;
using System.Data.Common;

namespace Dmarc.Common.Data
{
    public static class DbDataReaderExtensionMethods
    {
        public static DateTime GetDateTime(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.GetDateTime(index);
        }

        public static int GetInt32(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? 0 : reader.GetInt32(index);
        }

        public static long GetInt64(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? 0 : reader.GetInt64(index);
        }

        public static decimal GetDecimal(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? 0 : reader.GetDecimal(index);
        }

        public static string GetString(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? null : reader.GetString(index);
        }
    }
}