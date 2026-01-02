using Microsoft.Extensions.Caching.Memory;
using AutoPartsWarehouse.Data;

namespace AutoPartsWarehouse.Services
{
    public class CachedService<T> : ICachedService<T> where T : class
    {
        private readonly AutoPartsContext _db;
        private readonly IMemoryCache _cache;

        public CachedService(AutoPartsContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public void AddEntitiesToCache(string cacheKey, int rowsNumber = 20)
        {
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<T> items))
            {
                items = _db.Set<T>().Take(rowsNumber).ToList();

                // ИСПРАВЛЕНО: Вариант N = 13
                // Формула: 2 * 13 + 240 = 26 + 240 = 266 секунд
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(266));

                _cache.Set(cacheKey, items, cacheOptions);
            }
        }

        public IEnumerable<T> GetEntities(string cacheKey)
        {
            _cache.TryGetValue(cacheKey, out IEnumerable<T> items);
            return items ?? new List<T>();
        }
    }
}