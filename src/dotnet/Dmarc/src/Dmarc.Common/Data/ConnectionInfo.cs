namespace Dmarc.Common.Data
{
    public interface IConnectionInfo
    {
        string ConnectionString { get; }
    }

    public class StringConnectionInfo : IConnectionInfo
    {
        public StringConnectionInfo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }
    }
}