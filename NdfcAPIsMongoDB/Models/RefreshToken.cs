using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

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
