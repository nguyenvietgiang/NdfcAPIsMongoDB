using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class Title
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("sTitleName")]
        public string TitleName { get; set; }

        [BsonElement("sDes")]
        public string Description { get; set; }

        [BsonElement("sSeason")]
        public string Season { get; set; }
    }
}
