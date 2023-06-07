using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<Account> _accountCollection;
        private readonly IConfiguration _configuration;

        public AuthController(IMongoDatabase database, IConfiguration configuration)
        {
            _accountCollection = database.GetCollection<Account>("Account");
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var filter = Builders<Account>.Filter.Eq(a => a.Username, model.Username) &
                         Builders<Account>.Filter.Eq(a => a.Password, model.Password);

            var user = await _accountCollection.Find(filter).SingleOrDefaultAsync();

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("accountId", user.Id),
            new Claim("email", user.Email)
        };
                // Tạo JWT token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                // Cấu hình token
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                var response = new LoginResponse
                {
                    Id= user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = tokenString, 
                };
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            if (register == null)
            {
                return BadRequest();
            }
            // kiểm tra email
            var existingAccount = await _accountCollection.Find(x => x.Email == register.Email).FirstOrDefaultAsync();
            if (existingAccount != null)
            {
                return BadRequest("This Email has been used");
            }
            // tạo một register không bao gồm ID để mongoDB tự tạo
            var createAccount = new Account
            {
                Username = register.Username,
                Email = register.Email,
                Password = register.Password,
                Role = "User",
                Status = 1
            };

            await _accountCollection.InsertOneAsync(createAccount);
            return Ok(createAccount);
        }

        [HttpGet("userinfo")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            // Lấy thông tin người dùng từ HttpContext.User
            var userId = User.FindFirstValue("accountId");
            var role = User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            // Xử lý logic để trả về thông tin người dùng
            var userInfo = new
            {
                UserId = userId,
                Role = role
            };

            return Ok(userInfo);
        }
    }
}
