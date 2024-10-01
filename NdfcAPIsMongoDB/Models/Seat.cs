using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class Seat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string MatchId { get; set; }
        public string SeatNumber { get; set; }
        public bool Status { get; set; }
    }
}
