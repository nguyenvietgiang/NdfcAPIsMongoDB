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
        private readonly IConfiguration _configuration;

        public AuthController(IMongoDatabase database, IConfiguration configuration)
        {
            _accountCollection = database.GetCollection<Account>("Account");
            _configuration = configuration;
        }

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
                    };
                    return Ok(response);
                }
            }

            return Unauthorized();
        }
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
                client.Authenticate("nguyenvietgiang1110@gmail.com", "kholeeizbmxzykbs");
                client.Send(message);
                client.Disconnect(true);
            }
            return Ok("Mật khẩu đã được reset và gửi đến địa chỉ email đã đăng ký.");
        }
    }
}
