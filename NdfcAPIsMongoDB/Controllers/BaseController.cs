using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
public class BaseController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<BaseController> _logger;

    public BaseController(IMemoryCache cache, ILogger<BaseController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    protected TItem GetFromCache<TItem>(string cacheKey, Func<TItem> getItemCallback)
    {
        if (_cache.TryGetValue(cacheKey, out TItem item))
        {
            return item;
        }

        item = getItemCallback();

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));

        _cache.Set(cacheKey, item, cacheEntryOptions);

        return item;
    }

    protected void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

}
