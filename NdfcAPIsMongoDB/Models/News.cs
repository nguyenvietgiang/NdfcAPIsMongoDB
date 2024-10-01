using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class News
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("sTitle")]
        public string Title { get; set; }

        [BsonElement("sImg")]
        public string Image { get; set; }

        [BsonElement("sDesciption")]
        public string Description { get; set; }

        [BsonElement("sDetail")]
        public string Detail { get; set; }

        [BsonElement("dCreateOn")]
        public DateTime CreateOn { get; set; }

        [BsonElement("iStatus")]
        public int Status { get; set; }
    }
}
