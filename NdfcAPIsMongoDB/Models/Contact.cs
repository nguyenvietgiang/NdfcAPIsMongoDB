using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NdfcAPIsMongoDB.Models.DTO;

namespace NdfcAPIsMongoDB.Models
{
    public class Contact : ContactDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
