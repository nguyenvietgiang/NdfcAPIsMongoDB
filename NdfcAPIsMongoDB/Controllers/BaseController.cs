using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Common;
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
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(cacheKey, item, cacheEntryOptions);

        return item;
    }

    protected ActionResult<ApiResponse<T>> ApiOk<T>(T data, string message = "Thành công")
    {
        var response = new ApiResponse<T>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = message,
            Data = data
        };
        return Ok(response);
    }

    protected ActionResult ApiException(Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred.");
        var errorResponse = new ErrorResponse
        {
            ErrorMessage = ex.Message,
            StackTrace = ex.StackTrace
        };
        var response = new ApiResponse<ErrorResponse>
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = "Đã xảy ra lỗi",
            Data = errorResponse
        };
        return StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    protected void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

}
