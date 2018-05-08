using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySqlHelper = Dmarc.Common.Data.MySqlHelper;

namespace Dmarc.Common.TestSupport
{
    [TestFixture]
    [Category("Integration")]
    public abstract class DatabaseTestBase
    {
        private string _dataDirectory;
        private Process _process;

        private const string ConnectionStringBase = "Server = localhost; Port = 3306;";
        private const string ConnectionStringFull = ConnectionStringBase + "; Database = dmarc";

        [SetUp]
        protected virtual void SetUp()
        {
            _dataDirectory = CreateDataDirectory();

            _process = new Process();
            var arguments = new[]
            {
                "--standalone",
                "--console",
                $"--datadir={_dataDirectory}",
                "--skip-grant-tables",
                "--innodb_fast_shutdown=2",
                "--innodb_doublewrite=OFF",
            };

            _process.StartInfo.FileName = @"mysqld.exe";
            _process.StartInfo.Arguments = string.Join(" ", arguments);
            _process.Start();

            WaitForSuccessfulConnection();

            MySqlHelper.ExecuteNonQuery(ConnectionStringBase, "SET GLOBAL time_zone = '+00:00';");
            MySqlHelper.ExecuteNonQuery(ConnectionStringBase, "CREATE DATABASE IF NOT EXISTS dmarc;");

            string currentDirectory = Directory.GetCurrentDirectory();

            string sqlDirectory = Path.Combine(currentDirectory, @"..\..\..\..\..\..\sql\schema");

            DirectoryInfo directoryInfo = new DirectoryInfo(sqlDirectory);

            IOrderedEnumerable<FileInfo> sqlFiles = directoryInfo.GetFiles()
                .Where(_ => _.Extension == ".sql")
                .OrderBy(_ => _.Name);

            foreach (FileInfo file in sqlFiles)
            {
                string sql = File.ReadAllText(file.FullName);

                try
                {
                    MySqlHelper.ExecuteNonQuery(ConnectionStringFull, sql);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to execute {file.Name} with error:{System.Environment.NewLine}{e.Message}");
                }
            }
        }

        [TearDown]
        protected virtual void TearDown()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.WaitForExit();
                _process = null;
                DeleteDataDirectory(_dataDirectory);
            }
        }

        protected string ConnectionString => ConnectionStringFull;

        private string CreateDataDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            string dataDirectory = Path.Combine(currentDirectory, "MySqlData");
            Directory.CreateDirectory(dataDirectory);

            return dataDirectory;
        }

        private void DeleteDataDirectory(string dataDirectory)
        {
            if (Directory.Exists(dataDirectory))
            {
                Directory.Delete(dataDirectory, true);
            }
        }

        private void WaitForSuccessfulConnection()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionStringBase))
            {
                int maxRetryCount = 3;
                int retryCount = 0;
                do
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        Thread.Sleep(100);
                    }

                } while (connection.State != ConnectionState.Open && ++retryCount < maxRetryCount);

                connection.Close();
            }
        }
    }
}
