namespace NdfcAPIsMongoDB.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public TokenAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        // Kiểm tra header "Authorization" và lấy giá trị token
        string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Kiểm tra và xác thực token
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

                // Thiết lập các thông tin xác thực token
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                // Xác thực token và lấy thông tin người dùng từ token
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                // Lấy thông tin tài khoản từ token
                var accountId = principal.FindFirst("accountId")?.Value;
                var email = principal.FindFirst("email")?.Value;

                // Gán thông tin tài khoản vào context để sử dụng trong controller
                context.Items["AccountId"] = accountId;
                context.Items["Email"] = email;
            }
            catch (Exception)
            {
                // Xác thực token không thành công, xử lý tùy ý (ví dụ: trả về lỗi xác thực)
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        else
        {
            // Token không tồn tại, xử lý tùy ý (ví dụ: trả về lỗi xác thực)
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // Chuyển tiếp yêu cầu cho middleware tiếp theo trong chuỗi xử lý yêu cầu
        await _next(context);
    }
}
