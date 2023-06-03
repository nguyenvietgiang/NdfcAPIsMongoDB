using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("sSeason")]
        public string Season { get; set; }

        [BsonElement("lDetail")]
        public List<SeasonDetail> Detail { get; set; }
    }

    public class SeasonDetail
    {
        [BsonElement("iMonth")]
        public int Month { get; set; }

        [BsonElement("iTotal_matches")]
        public int TotalMatches { get; set; }

        [BsonElement("iTotal_wins")]
        public int TotalWins { get; set; }

        [BsonElement("iTotal_losses")]
        public int TotalLosses { get; set; }

        [BsonElement("iTotal_draws")]
        public int TotalDraws { get; set; }

        [BsonElement("iGoals")]
        public int Goals { get; set; }

        [BsonElement("sTop_scorer")]
        public string TopScorer { get; set; }

        [BsonElement("yellow_cards")]
        public int YellowCards { get; set; }

        [BsonElement("red_cards")]
        public int RedCards { get; set; }
    }
}
