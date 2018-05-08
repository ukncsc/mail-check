using System;
using System.Text.RegularExpressions;

namespace Dmarc.Common.Data
{
    public class DatabaseAccessUtils
    {
        public static string GetDatabaseUsername(string connectionString)
        {
            string pattern = @"Uid =(.*?)\;";

            var match = Regex.Match(connectionString, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            else
            {
                throw new Exception("Unable to find database username in the connection string");
            }

        }
    }
}
