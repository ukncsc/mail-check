using System;
using System.Data.Common;
using System.IO;

namespace Dmarc.Common.Data
{
    public static class DbDataReaderExtensionMethods
    {
        public static DateTime GetDateTime(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.GetDateTime(index);
        }

        public static DateTime? GetDateTimeNullable(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);
        }

        public static int GetInt32(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? 0 : reader.GetInt32(index);
        }

        public static short GetInt16(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? (short)0  : reader.GetInt16(index);
        }

        public static int? GetInt32Nullable(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? (int?)null : reader.GetInt32(index);
        }

        public static long GetInt64(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? 0 : reader.GetInt64(index);
        }

        public static ulong GetUInt64(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? 0 : (ulong)reader.GetInt64(index);
        }

        public static ulong? GetUInt64Nullable(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? (ulong?)null : (ulong)reader.GetInt64(index);
        }

        public static long? GetInt64Nullable(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? (long?)null : reader.GetInt64(index);
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

        public static bool GetBoolean(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return !reader.IsDBNull(index) && reader.GetBoolean(index);
        }

        public static Stream GetStream(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? null : reader.GetStream(index);
        }

        public static byte[] GetByteArray(this DbDataReader reader, string name)
        {
            using (Stream stream = reader.GetStream(name))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            
        }

        public static ushort GetUInt16(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? default(ushort) : (ushort)reader.GetInt16(index);
        }

        public static ushort? GetUInt16Nullable(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);

            return reader.IsDBNull(index) ? (ushort?)null : (ushort)reader.GetInt16(index);
        }

        public static bool IsDbNull(this DbDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name));
        }
    }
}