using System.Diagnostics.CodeAnalysis;
using Bruno57.Domain.Factory.Abstractions;
using Bruno57.Domain.Foundations.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Bruno57.Domain.Factory.CacheService;

public sealed class CacheProvider : ICacheProvider
{
    private readonly IMemoryCache _memoryCache;
    
    public CacheProvider(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache.CheckForNull();
    }

    public object Get(object key)
    {
        if (key.ToString() == string.Empty)
        {
            return string.Empty;
        }

        var getValueResult = _memoryCache.TryGetValue(key, out var value);

        if (value is null || !getValueResult)
        {
            return string.Empty;
        }

        return value;
    }

    public object Insert(object key, object value)
    {
        if (key.ToString() == string.Empty 
            || value.ToString() == string.Empty)
        {
            return string.Empty;
        }

        return _memoryCache.Set(key, value);
    }

    [ExcludeFromCodeCoverage]
    public object Insert(object key, object value, TimeSpan timeTillExpires)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public object Insert(object key, object value, DateTime absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public object Remove(object key)
    {
        throw new NotImplementedException();
    }
}
