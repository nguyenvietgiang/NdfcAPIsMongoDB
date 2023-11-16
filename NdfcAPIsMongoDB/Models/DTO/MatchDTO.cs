using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models.DTO
{
    public class MatchDTO
    {
        [BsonElement("sEnemy")]
        public string Enemy { get; set; }

        [BsonElement("sStadium")]
        public string Stadium { get; set; }

        [BsonElement("dTime")]
        public DateTime Time { get; set; }

        [BsonElement("sLeague")]
        public string League { get; set; }
    }
}
