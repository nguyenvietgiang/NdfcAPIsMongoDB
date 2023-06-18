using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NdfcAPIsMongoDB.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime SentTime { get; set; }
    }
}
