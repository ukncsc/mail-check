using System.Threading.Tasks;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Caching.Redis;
//using Microsoft.Extensions.Options;

namespace Dmarc.MxSecurityTester.Util
{
    //public interface IDistributedCacheWrapper : IDistributedCache
    //{
    //    Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options);
    //    Task<string> GetStringAsync(string key);
    //}

    //public class DistributedCacheWrapper : RedisCache, IDistributedCacheWrapper
    //{
    //    public DistributedCacheWrapper(IOptions<RedisCacheOptions> optionsAccessor) 
    //        : base(optionsAccessor){}

    //    public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options)
    //    {
    //        return DistributedCacheExtensions.SetStringAsync(this, key, value, options);
    //    }

    //    public Task<string> GetStringAsync(string key)
    //    {
    //        return DistributedCacheExtensions.GetStringAsync(this, key);
    //    }
    //}
}