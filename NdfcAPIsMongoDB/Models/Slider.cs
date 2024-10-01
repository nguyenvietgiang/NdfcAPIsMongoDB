using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class Slider
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("sTitle")]
        public string Title { get; set; }

        [BsonElement("sImg")]
        public string Img { get; set; }

        [BsonElement("sLink")]
        public string Link { get; set; }

        [BsonElement("iStatus")]
        public int Status { get; set; }
    }
}
