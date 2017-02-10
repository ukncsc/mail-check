using System.Reflection;
using MySql.Data.MySqlClient;
using SimpleMigrations;
using SimpleMigrations.Console;
using SimpleMigrations.DatabaseProvider;

namespace Dmarc.Migrations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly;
            var connectionString = @"Server=; Port=; Database=; Uid = user; Pwd = pass;";
            using (var connection = new MySqlConnection(connectionString))
            {
                var databaseProvider = new MysqlDatabaseProvider(connection);
                var migrator = new SimpleMigrator(migrationsAssembly, databaseProvider);

                ConsoleRunner consoleRunner = new ConsoleRunner(migrator);
                consoleRunner.Run(args);
            }
        }
    }
}
