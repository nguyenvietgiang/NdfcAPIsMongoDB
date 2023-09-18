using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models.DTO
{
    public class SubcriberDTO
    {
        [BsonElement("sName")]
        public string Name { get; set; }

        [BsonElement("sEmail")]
        public string Email { get; set; }
    }
}
