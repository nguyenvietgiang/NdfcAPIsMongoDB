using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models.DTO
{
    public class NewsDTO
    {
        [BsonElement("sTitle")]
        public string Title { get; set; }

        [BsonElement("sImg")]
        public IFormFile Image { get; set; }

        [BsonElement("sDesciption")]
        public string Description { get; set; }

        [BsonElement("sDetail")]
        public string Detail { get; set; }
    }
}
