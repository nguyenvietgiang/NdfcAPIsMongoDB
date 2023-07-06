using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;
using System.Security.Claims;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IMongoCollection<ChatMessage> _chatMessages;

        public ChatController(IHubContext<ChatHub> chatHubContext, IMongoDatabase database)
        {
            _chatHubContext = chatHubContext;
            _chatMessages = database.GetCollection<ChatMessage>("ChatMessages");
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _chatMessages.Find(_ => true).ToListAsync();

            var connectionId = HttpContext.Connection.Id;
            await _chatHubContext.Clients.Client(connectionId).SendAsync("ReceiveMessages", messages);

            return Ok(messages);
        }

        [HttpDelete("messages/{messageId}")]
        [Authorize(Roles = "Admin")] // chỉ có admin mới được xóa
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
           
            var deleteResult = await _chatMessages.DeleteOneAsync(message => message.Id == messageId);

            if (deleteResult.DeletedCount > 0)
            {
                await _chatHubContext.Clients.All.SendAsync("MessageDeleted", messageId);
            }

            return Ok();
        }


        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lưu tin nhắn vào cơ sở dữ liệu
            var chatMessage = new ChatMessage
            {
                SenderName = User.FindFirstValue(ClaimTypes.Name),
                Message = request.Message,
                SentTime = DateTime.UtcNow
            };
            await _chatMessages.InsertOneAsync(chatMessage);

            // Gửi tin nhắn tới các client khác
            await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", chatMessage);

            return Ok();
        }
    }

    public class ChatMessageRequest
    {
        public string Message { get; set; }
    }
}

