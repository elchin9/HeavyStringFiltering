using HeavyStringFiltering.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HeavyStringFiltering.Infrastructure.Storage
{
    public class TextStorageCache : ITextStorageCache
    {
        private readonly IMemoryCache _cache;
        public TextStorageCache(IMemoryCache cache) => _cache = cache;

        public void Save(Guid id, string result)
        {
            _cache.Set(id, result, TimeSpan.FromMinutes(30));
        }
    }
}
