using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Common
{
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<ChatMessage> _chatMessages;

        public ChatHub(IMongoDatabase database)
        {
            _chatMessages = database.GetCollection<ChatMessage>("ChatMessages");
        }

        public async override Task OnConnectedAsync()
        {
            // Logic xử lý khi có client kết nối
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            // Logic xử lý khi có client ngắt kết nối
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string senderName, string message)
        {
            var chatMessage = new ChatMessage
            {
                SenderName = senderName,
                Message = message,
                SentTime = DateTime.UtcNow
            };

            // Lưu tin nhắn vào MongoDB
            await _chatMessages.InsertOneAsync(chatMessage);

            // Gửi tin nhắn tới tất cả các client khác nhau
            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }

        public async Task GetMessages()
        {
            var messages = await _chatMessages.Find(_ => true).ToListAsync();
            await Clients.Caller.SendAsync("ReceiveMessages", messages);
        }

        public async Task DeleteMessage(string messageId)
        {
            var objectId = ObjectId.Parse(messageId);
            var deleteResult = await _chatMessages.DeleteOneAsync(message => message.Id == messageId);

            if (deleteResult.DeletedCount > 0)
            {
                await Clients.All.SendAsync("MessageDeleted", objectId);
            }
        }

    }
}

