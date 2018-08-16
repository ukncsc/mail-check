using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Dmarc.Common.Data
{
    public static class MySqlParametersCollectionExtensionMethods
    {
        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, string value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.String
            };
            collection.Add(parameter);
        }


        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, byte[] value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.Binary
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, int? value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.Int32
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, int value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.Int32
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, long? value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.Int64
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, long value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.Int64
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, ulong? value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.UInt64
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, ulong value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.UInt64
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, DateTime? value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.DateTime
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, DateTime value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.DateTime
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, bool value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.Boolean
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, ushort value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.UInt16
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, ushort? value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.UInt16
            };
            collection.Add(parameter);
        }

        public static void AddWithValue(this MySqlParameterCollection collection, string parameterName, double? value)
        {
            MySqlParameter parameter = new MySqlParameter
            {
                Value = value,
                ParameterName = parameterName,
                DbType = DbType.Double
            };
            collection.Add(parameter);
        }
    }
}