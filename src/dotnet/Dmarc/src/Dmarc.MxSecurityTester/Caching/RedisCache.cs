using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using StackExchange.Redis;

namespace Dmarc.MxSecurityTester.Caching
{
    public class RedisCache : ICache, IDisposable
    {
        private const int RedisPort = 6379;

        private readonly IRedisConfig _config;
        private readonly ILogger _log;

        private volatile ConnectionMultiplexer _connection;
        private IDatabase _cache;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public RedisCache(IRedisConfig config, ILogger log)
        {
            _config = config;
            _log = log;
        }

        public async Task<string> GetString(string key)
        {
            await Connect();
            RedisValue value = await _cache.StringGetAsync(key);
            return value;
        }

        public async Task SetString(string key, string value, TimeSpan ttl)
        {
            await Connect();
            await _cache.StringSetAsync(key, value, ttl);
        }

        private async Task Connect()
        {
            if (_connection != null)
            {
                return;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (_connection == null)
                {
                    IPAddress[] addresses = await Dns.GetHostAddressesAsync(_config.CacheHostName);
                    string connectionString = string.Join(",", addresses.Select(x => $"{x.MapToIPv4().ToString()}:{RedisPort}"));

                    _connection = ConnectionMultiplexer.Connect(connectionString);

                    _log.Debug($"Successfully connected to redis: {connectionString}");

                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            _connection?.Close();
        }
    }
}