using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models.DTO
{
    public class Register
    {
        [BsonElement("sName")]
        public string Username { get; set; }

        [BsonElement("sEmail")]
        public string Email { get; set; }

        [BsonElement("sPassword")]
        public string Password { get; set; }
    }
}
