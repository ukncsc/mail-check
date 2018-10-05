using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Dmarc.Common.Data
{
    public static class Db
    {
        public static Task<T> ExecuteReaderTimed<T>(IConnectionInfoAsync connectionInfo,
            string sql, Action<MySqlParameterCollection> addParameters, Func<DbDataReader, Task<T>> factory,
            Action<string> log, string callName) where T : class
        {
            using (Timer timer = new Timer(log, callName))
            {
                return ExecuteReader(connectionInfo, sql, addParameters, factory);
            }
        }

        public static async Task<T> ExecuteReader<T>(IConnectionInfoAsync connectionInfo, string sql,
               Action<MySqlParameterCollection> addParameters, Func<DbDataReader, Task<T>> factory) where T : class
        {
            string connectionString = await connectionInfo.GetConnectionStringAsync();
            return await ExecuteReader(connectionString, sql, addParameters, factory);
        }

        public static Task<T> ExecuteReader<T>(string connectionString, string sql,
            Action<MySqlParameterCollection> addParameters, Func<DbDataReader, Task<T>> factory) where T : class
        {
            Func<MySqlCommand, Task<T>> executeCommand = async command =>
            {
                T result;
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    result = await factory(reader);
                }
                return result;
            };

            return Execute(connectionString, sql, addParameters, executeCommand);
        }

        public static async Task<T> ExecuteReaderSingleResultTimed<T>(IConnectionInfoAsync connectionInfo,
           string sql, Action<MySqlParameterCollection> addParameters, Func<DbDataReader, T> factory,
           Action<string> log, string callName) where T : class
        {
            using (Timer timer = new Timer(log, callName))
            {
                string connectionString = await connectionInfo.GetConnectionStringAsync();
                return await ExecuteReaderSingleResult(connectionString, sql, addParameters, factory);
            }
        }

        public static async Task<T> ExecuteReaderSingleResult<T>(IConnectionInfoAsync connectionInfo,
            string sql, Action<MySqlParameterCollection> addParameters, Func<DbDataReader, T> factory) where T : class
        {
            string connectionString = await connectionInfo.GetConnectionStringAsync();
            return await ExecuteReaderSingleResult(connectionString, sql, addParameters, factory);
        }

        public static async Task<T> ExecuteReaderSingleResult<T>(string connectionString, string sql,
            Action<MySqlParameterCollection> addParameters, Func<DbDataReader, T> factory) where T : class
        {
            Func<DbDataReader, Task<T>> iteratedFactory = async _ =>
           {
               T result = null;
               while (await _.ReadAsync())
               {
                   result = factory(_);
               }
               return result;
           };

            return await ExecuteReader(connectionString, sql, addParameters, iteratedFactory);
        }

        public static async Task<List<T>> ExecuteReaderListResultTimed<T>(IConnectionInfoAsync connectionInfo,
            string sql, Action<MySqlParameterCollection> addParameters, Func<DbDataReader, T> factory,
            Action<string> log, string callName)
        {
            using (Timer timer = new Timer(log, callName))
            {
                string connectionString = await connectionInfo.GetConnectionStringAsync();
                return await ExecuteReaderListResult(connectionString, sql, addParameters, factory);
            }
        }

        public static async Task<List<T>> ExecuteReaderListResult<T>(IConnectionInfoAsync connectionInfo, string sql,
            Action<MySqlParameterCollection> addParameters, Func<DbDataReader, T> factory)
        {
            string connectionString = await connectionInfo.GetConnectionStringAsync();
            return await ExecuteReaderListResult(connectionString, sql, addParameters, factory);
        }

        public static async Task<List<T>> ExecuteReaderListResult<T>(string connectionString, string sql,
            Action<MySqlParameterCollection> addParameters, Func<DbDataReader, T> factory)
        {
            Func<DbDataReader, Task<List<T>>> iteratedFactory = async _ =>
            {
                List<T> results = new List<T>();
                while (await _.ReadAsync())
                {
                    results.Add(factory(_));
                }
                return results;
            };

            return await ExecuteReader(connectionString, sql, addParameters, iteratedFactory);
        }

        public static async Task<T> ExecuteScalarTimed<T>(IConnectionInfoAsync connectionInfo, string sql,
            Action<MySqlParameterCollection> addParameters, Action<string> log, string callName)
        {
            using (Timer timer = new Timer(log, callName))
            {
                return await ExecuteScalar(connectionInfo, sql, addParameters, (Func<object, T>)null);
            }
        }

        public static async Task<T> ExecuteScalarTimed<T>(IConnectionInfoAsync connectionInfo, string sql,
            Action<MySqlParameterCollection> addParameters, Func<object, T> factory, Action<string> log, string callName)
        {
            using (Timer timer = new Timer(log, callName))
            {
                return await ExecuteScalar(connectionInfo, sql, addParameters, factory);
            }
        }

        public static async Task<T> ExecuteScalar<T>(IConnectionInfoAsync connectionInfo, string sql,
            Action<MySqlParameterCollection> addParameters, Func<object, T> factory = null)
        {
            string connectionString = await connectionInfo.GetConnectionStringAsync();
            return await ExecuteScalar(connectionString, sql, addParameters, factory);
        }

        public static Task<T> ExecuteScalar<T>(string connectionString, string sql,
            Action<MySqlParameterCollection> addParameters, Func<object, T> factory = null)
        {
            if (factory == null)
            {
                factory = o => (T)o;
            }

            Func<MySqlCommand, Task<T>> executeCommand = async command => factory(await command.ExecuteScalarAsync());

            return Execute(connectionString, sql, addParameters, executeCommand);
        }

        private static async Task<T> Execute<T>(string connectionString, string sql,
            Action<MySqlParameterCollection> addParameters, Func<MySqlCommand, Task<T>> executeCommand)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    addParameters(command.Parameters);

                    command.Prepare();

                    T result = await executeCommand(command);

                    connection.Close();

                    return result;
                }
            }
        }
    }
}
