using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NdfcAPIsMongoDB.Models
{
    public class Subscriber
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("sName")]
        public string Name { get; set; }

        [BsonElement("sEmail")]
        public string Email { get; set; }

    }
}
