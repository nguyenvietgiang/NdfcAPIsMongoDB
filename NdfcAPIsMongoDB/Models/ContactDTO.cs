using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class ContactDTO
    {

        [BsonElement("sName")]
        public string Name { get; set; }

        [BsonElement("sEmail")]
        public string Email { get; set; }

        [BsonElement("sTopic")]
        public string Topic { get; set; }

        [BsonElement("sDetail")]
        public string Detail { get; set; }
    }
}
