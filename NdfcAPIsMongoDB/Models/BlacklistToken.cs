using MongoDB.Bson;

namespace NdfcAPIsMongoDB.Models
{
    public class BlacklistToken
    {
        public ObjectId Id { get; set; }
        public string? Token { get; set; }
    }
}
