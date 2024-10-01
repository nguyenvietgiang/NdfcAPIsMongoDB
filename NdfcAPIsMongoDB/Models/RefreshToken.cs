using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class RefreshToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
