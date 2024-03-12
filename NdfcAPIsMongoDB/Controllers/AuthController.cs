using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using MimeKit;
using MailKit.Net.Smtp;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<Account> _accountCollection;
        private readonly IMongoCollection<RefreshToken> _refreshTokensCollection;
        private readonly IMongoCollection<BlacklistToken> _blacklistTokensCollection; // Đối tượng danh sách đen
        private readonly IConfiguration _configuration;

        public AuthController(IMongoDatabase database, IConfiguration configuration)
        {
            _accountCollection = database.GetCollection<Account>("Account");
            _refreshTokensCollection = database.GetCollection<RefreshToken>("RefreshToken");
            _configuration = configuration;
        }

        /// <summary>
        /// login and get token and refresh token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var filter = Builders<Account>.Filter.Eq(a => a.Username, model.Username);
            var user = await _accountCollection.Find(filter).SingleOrDefaultAsync();
            if (user != null)
            {
                // Mã hóa mật khẩu đăng nhập bằng MD5
                string hashedPassword;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(model.Password);
                    byte[] hashedBytes = md5.ComputeHash(passwordBytes);
                    hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
                // So sánh mật khẩu đã mã hóa với mật khẩu đã lưu trữ
                if (hashedPassword == user.Password)
                {
                    // Tạo refresh token
                    var refreshToken = new RefreshToken
                    {
                        Token = GenerateRefreshToken(),
                        UserId = user.Id,
                        ExpiresAt = DateTime.Now.AddDays(1)
                    };

                    // Lưu trữ refresh token vào cơ sở dữ liệu
                    await _refreshTokensCollection.InsertOneAsync(refreshToken);

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
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Role = user.Role,
                        Token = tokenString,
                        RefreshToken = refreshToken.Token
                    };
                    return Ok(response);
                }
            }

            return Unauthorized();
        }

        /// <summary>
        /// get new access token by refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var filter = Builders<RefreshToken>.Filter.And(
                Builders<RefreshToken>.Filter.Eq(rt => rt.Token, request.RefreshToken),
                Builders<RefreshToken>.Filter.Gte(rt => rt.ExpiresAt, DateTime.Now)
            );

            var refreshToken = await _refreshTokensCollection.Find(filter).SingleOrDefaultAsync();
            if (refreshToken == null)
            {
                return Unauthorized();
            }

            // Tạo mới token và refresh token
            var user = await _accountCollection.Find(Builders<Account>.Filter.Eq(a => a.Id, refreshToken.UserId))
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("accountId", user.Id),
        new Claim("email", user.Email)
    };

            // Tạo JWT token mới
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Cấu hình token mới
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Cập nhật refresh token mới
            var newRefreshToken = new RefreshToken
            {
                Id = refreshToken.Id,
                Token = GenerateRefreshToken(),
                UserId = refreshToken.UserId,
                ExpiresAt = DateTime.Now.AddDays(1)
            };

            await _refreshTokensCollection.ReplaceOneAsync(Builders<RefreshToken>.Filter.Eq(rt => rt.Token, refreshToken.Token),
                newRefreshToken);

            var response = new RefreshTokenResponse
            {
                Token = tokenString,
                RefreshToken = newRefreshToken.Token
            };

            return Ok(response);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        /// <summary>
        /// logout and remove token into blacklist
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var accessToken = HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            // Kiểm tra và thêm access token vào danh sách đen
            if (!string.IsNullOrEmpty(accessToken) && _blacklistTokensCollection != null)
            {
                await AddToBlacklistAsync(accessToken);
                return Ok(new { message = "Logout successful" });
            }

            return BadRequest(new { message = "Access token not found" });
        }


        private async Task AddToBlacklistAsync(string token)
        {
            if (_blacklistTokensCollection == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            var filter = Builders<BlacklistToken>.Filter.Eq(t => t.Token, token);
            var existingToken = await _blacklistTokensCollection.Find(filter).FirstOrDefaultAsync();
            if (existingToken == null)
            {
                var blacklistToken = new BlacklistToken
                {
                    Token = token,
                };
                await _blacklistTokensCollection.InsertOneAsync(blacklistToken);
            }
        }


        /// <summary>
        /// create new account
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            if (register == null)
            {
                return BadRequest();
            }
            // kiểm tra email và tên
            var existingAccount = await _accountCollection.Find(x => x.Email == register.Email || x.Username == register.Username).FirstOrDefaultAsync();
            if (existingAccount != null)
            {
                if (existingAccount.Email == register.Email)
                {
                    return BadRequest("This Email has been used");
                }
                else if (existingAccount.Username == register.Username)
                {
                    return BadRequest("This Name has been used");
                }
            }
            // Mã hóa mật khẩu bằng MD5
            string hashedPassword;
            using (MD5 md5 = MD5.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(register.Password);
                byte[] hashedBytes = md5.ComputeHash(passwordBytes);
                hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            // tạo một register không bao gồm ID để mongoDB tự tạo
            var createAccount = new Account
            {
                Username = register.Username,
                Email = register.Email,
                Password = hashedPassword,
                Role = "User",
                Status = 1
            };

            await _accountCollection.InsertOneAsync(createAccount);
            return Ok(createAccount);
        }

        /// <summary>
        /// get current user infor
        /// </summary>
        [HttpGet("userinfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            // Lấy thông tin người dùng từ HttpContext.User
            var userIdClaim = User.FindFirst("accountId");

            if (userIdClaim == null)
            {
                return BadRequest("Không tìm thấy thông tin người dùng.");
            }
            var userId = userIdClaim.Value;
            var objectId = ObjectId.Parse(userId);
            var filter = Builders<Account>.Filter.Eq("_id", objectId);
            var user = await _accountCollection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            return Ok(user);
        }

        /// <summary>
        /// update user information
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccount(string id, Register register)
        {
            if (register == null)
            {
                return BadRequest();
            }
            var account = new Account
            {
                Id = id,
                Username = register.Username,
                Email = register.Email,
                Role = "User",
                Password = register.Password,
                Status = 1
            };
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Account>.Filter.Eq("_id", objectId);
            var result = await _accountCollection.ReplaceOneAsync(filter, account);
            return Ok(account);
        }

        /// <summary>
        /// delete many account by list id
        /// </summary>
        [HttpDelete("list-accounts")]
        public async Task<IActionResult> DeleteAccounts(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("List of IDs is required.");
            }

            foreach (var id in ids)
            {
                var objectId = ObjectId.Parse(id);
                var filter = Builders<Account>.Filter.Eq("_id", objectId);
                var result = await _accountCollection.DeleteOneAsync(filter);
            }
           return Ok();
        }

        /// <summary>
        /// patch account 
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAccount(string id,JsonPatchDocument<Account> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var account = await _accountCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (account == null)
            {
                return NotFound();
            }

            try
            {
                patchDocument.ApplyTo(account);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("PatchError", ex.Message);
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _accountCollection.ReplaceOneAsync(x => x.Id == id, account);

            return Ok(account);
        }

        /// <summary>
        /// change password of current user
        /// </summary>
        [HttpPut("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userIdClaim = User.FindFirst("accountId");

            if (userIdClaim == null)
            {
                return BadRequest("Không tìm thấy thông tin người dùng.");
            }
            var userId = userIdClaim.Value;
            var objectId = ObjectId.Parse(userId);
            var filter = Builders<Account>.Filter.Eq("_id", objectId);
            var user = await _accountCollection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }
            // Mã hóa mật khẩu hiện tại để so sánh với mật khẩu đã lưu trữ
            string hashedCurrentPassword;
            using (MD5 md5 = MD5.Create())
            {
                byte[] currentPasswordBytes = Encoding.UTF8.GetBytes(model.CurrentPassword);
                byte[] currentHashedBytes = md5.ComputeHash(currentPasswordBytes);
                hashedCurrentPassword = BitConverter.ToString(currentHashedBytes).Replace("-", "").ToLower();
            }
            if (hashedCurrentPassword != user.Password)
            {
                return BadRequest("Mật khẩu hiện tại không chính xác.");
            }
            // Mã hóa mật khẩu mới
            string hashedNewPassword;
            using (MD5 md5 = MD5.Create())
            {
                byte[] newPasswordBytes = Encoding.UTF8.GetBytes(model.NewPassword);
                byte[] newHashedBytes = md5.ComputeHash(newPasswordBytes);
                hashedNewPassword = BitConverter.ToString(newHashedBytes).Replace("-", "").ToLower();
            }
            // Lưu trữ mật khẩu mới vào cơ sở dữ liệu
            user.Password = hashedNewPassword;
            await _accountCollection.ReplaceOneAsync(filter, user);
            return Ok("Mật khẩu đã được thay đổi.");
        }
        // tạo chuỗi mật khẩu ngẫu nhiên
        private string GenerateRandomPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder passwordBuilder = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 8; i++)
            {
                int randomIndex = random.Next(0, validChars.Length);
                passwordBuilder.Append(validChars[randomIndex]);
            }

            return passwordBuilder.ToString();
        }


        /// <summary>
        /// fogot password and take email new password
        /// </summary>
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            // Tìm tài khoản dựa trên địa chỉ email
            var filter = Builders<Account>.Filter.Eq(a => a.Email, model.Email);
            var user = await _accountCollection.Find(filter).SingleOrDefaultAsync();
            if (user == null)
            {
                // Không tìm thấy tài khoản với địa chỉ email này
                return BadRequest("Email không tồn tại.");
            }

            // Tạo mật khẩu mới ngẫu nhiên
            string newPassword = GenerateRandomPassword();

            // Mã hóa mật khẩu mới
            string hashedNewPassword;
            using (MD5 md5 = MD5.Create())
            {
                byte[] newPasswordBytes = Encoding.UTF8.GetBytes(newPassword);
                byte[] newHashedBytes = md5.ComputeHash(newPasswordBytes);
                hashedNewPassword = BitConverter.ToString(newHashedBytes).Replace("-", "").ToLower();
            }
            // Cập nhật mật khẩu mới vào cơ sở dữ liệu
            user.Password = hashedNewPassword;
            await _accountCollection.ReplaceOneAsync(filter, user);
            // Gửi email chứa mật khẩu mới đến địa chỉ email đã đăng ký
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("nguyenvietgiang1110@gmail.com"));
            message.To.Add(MailboxAddress.Parse(user.Email));
            message.Subject = "Password Reset";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
        <html>
            <head>
                <title>Password Reset</title>
            </head>
            <body>
                <h2>Password Reset</h2>
                <p>Your new password is: {newPassword}</p>
                <p>Please change your password after logging in.</p>
            </body>
        </html>";
            message.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("nguyenvietgiang1110@gmail.com", "********");
                client.Send(message);
                client.Disconnect(true);
            }
            return Ok("Mật khẩu đã được reset và gửi đến địa chỉ email đã đăng ký.");
        }

        /// <summary>
        /// admin get list account
        /// </summary>
        [HttpGet("list-accounts")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListAccounts()
        {
            var accounts = await _accountCollection.Find(_ => true).ToListAsync();
            return Ok(accounts);
        }

        /// <summary>
        /// admin change user status
        /// </summary>
        [HttpPut("change-status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Account>.Filter.Eq("_id", objectId);
            var account = await _accountCollection.Find(filter).FirstOrDefaultAsync();

            if (account == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            int newStatus = account.Status == 0 ? 1 : 0;

            var update = Builders<Account>.Update.Set("Status", newStatus);
            var result = await _accountCollection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            return Ok("Trạng thái người dùng đã được thay đổi.");
        }

    }
}
