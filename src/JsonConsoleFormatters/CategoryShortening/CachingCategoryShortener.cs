using System.Collections;

namespace JsonConsoleFormatters.CategoryShortening;

internal class CachingCategoryShortener : ICategoryShortener
{
    // see https://www.hanselman.com/blog/differences-between-hashtable-vs-dictonary-vs-concurrentdictionary-vs-immutabledictionary
    private readonly Hashtable _cache = new();
    private readonly ICategoryShortener _shortener;

    public CachingCategoryShortener(ICategoryShortener shortener)
    {
        _shortener = shortener;
    }

    public ReadOnlySpan<byte> Shorten(string category)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        var result = (byte[]?)_cache[category];
        if (result is null)
        {
            result = _shortener.Shorten(category).ToArray();
            lock (_cache.SyncRoot)
            {
                _cache.Add(category, result);
            }
        }

        return result;
    }
}
