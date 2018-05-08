using System;
using System.Threading.Tasks;

namespace Dmarc.MxSecurityTester.Caching
{
    public interface ICache
    {
        Task<string> GetString(string key);
        Task SetString(string key, string value, TimeSpan ttl);
    }
}