using System;

namespace NA.PMS.Common.Caching
{
    /// <summary>
    ///     Extensions
    /// </summary>
    public static class CacheExtensions
    {

        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 300, acquire);
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            else
            {
                var result = acquire();

                cacheManager.Set(key, result, cacheTime);
                return result;
            }
        }
    }
}