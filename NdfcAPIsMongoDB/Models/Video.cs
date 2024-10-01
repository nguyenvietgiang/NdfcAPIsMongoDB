using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class Video
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("sTitle")]
        public string Title { get; set; }

        [BsonElement("sVideo")]
        public string sVideo { get; set; }

        [BsonElement("dCreateOn")]
        public DateTime CreateOn { get; set; }

        [BsonElement("iStatus")]
        public int Status { get; set; }
    }
}
