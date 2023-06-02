using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NdfcAPIsMongoDB.Models
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("sName")]
        public string Username { get; set; }

        [BsonElement("sEmail")]
        public string Email { get; set; }

        [BsonElement("sPassword")]
        public string Password { get; set; }

        [BsonElement("sRole")]
        public string Role { get; set; }

        [BsonElement("iStatus")]
        public int Status { get; set; }
    }
}
