using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dmarc.Common.Encryption;

namespace Dmarc.Common.Data
{
    public interface IConnectionInfoAsync
    {
        Task<string> GetConnectionStringAsync();
    }

    public class ConnectionInfoAsync : IConnectionInfoAsync
    {
        private readonly IParameterStoreRequest _parameterStoreRequest;

        private readonly string _connectionString;
        private const string StringToReplace = "Pwd = ";


        public ConnectionInfoAsync(IParameterStoreRequest parameterStoreRequest, IConnectionInfo connectionString)
        {
            _parameterStoreRequest = parameterStoreRequest;
            _connectionString = connectionString.ConnectionString;
        }

        public async Task<string> GetConnectionStringAsync()
        {
            if (ShouldAppendString(_connectionString))
            {  
               
                string username = GetDatabaseUsername(_connectionString);
                string password = await _parameterStoreRequest.GetParameterValue(username);
            
                StringBuilder sb = new StringBuilder(_connectionString);
                sb.Append(StringToReplace).Append(password).Append(";");

                return sb.ToString();
            }
            return _connectionString;
        }

        private bool ShouldAppendString(string connectionString)
        {
            return !connectionString.Contains(StringToReplace);
        }

        private string GetDatabaseUsername(string connectionString)
        {
            string pattern = @"Uid =(.*?)\;";

            var match = Regex.Match(connectionString, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            throw new Exception("Unable to find database username in the connection string");
        }
    }
}
