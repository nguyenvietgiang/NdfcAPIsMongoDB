using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models.DTO
{
    public class LeagueDTO
    {
        [BsonElement("sName")]
        public string Name { get; set; }

        [BsonElement("sReward")]
        public string Reward { get; set; }

        [BsonElement("sYear")]
        public string Year { get; set; }

    }
}
