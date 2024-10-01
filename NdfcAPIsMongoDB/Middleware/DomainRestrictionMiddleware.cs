public class DomainRestrictionMiddleware
{
    private readonly RequestDelegate _next;

    public DomainRestrictionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Kiểm tra tên miền trong yêu cầu
        var requestHost = context.Request.Host.Host;
        var requestPath = context.Request.Path;

        string[] restrictedDomains = { "http://localhost:4200" };
        string[] restrictedPaths = { "/v1/api/" };

        if (restrictedDomains.Contains(requestHost) && restrictedPaths.Any(path => requestPath.StartsWithSegments(path)))
        {
            context.Response.StatusCode = 403; // HTTP 403 - Forbidden
            await context.Response.WriteAsync("Access Forbidden: Miền này đã bị chặn bởi NDFC.");
        }
        else
        {
            await _next(context);
        }
    }
}
