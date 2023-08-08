using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class ClubHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("sName")]
        public string Name { get; set; }

        [BsonElement("sIcon")]
        public string IconUrl { get; set; }

        [BsonElement("sDetail")]
        public string Detail { get; set; }

        [BsonElement("dFound")]
        public DateTime FoundedDate { get; set; }

        [BsonElement("sLocation")]
        public string Location { get; set; }

    }
}
