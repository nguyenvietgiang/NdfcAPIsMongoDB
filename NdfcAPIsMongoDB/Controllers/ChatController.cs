using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models.DTO;

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

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lưu tin nhắn vào cơ sở dữ liệu
            var chatMessage = new ChatMessage
            {
                SenderName = request.SenderName,
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
        public string SenderName { get; set; }
        public string Message { get; set; }
    }
}

