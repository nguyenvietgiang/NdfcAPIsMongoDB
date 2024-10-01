namespace NdfcAPIsMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Player
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("sName")]
    public string Name { get; set; }

    [BsonElement("iAge")]
    public int Age { get; set; }

    [BsonElement("sRole")]
    public string Role { get; set; }

    [BsonElement("sPosition")]
    public string Position { get; set; }

    public string sImg { get; set; } // Đường dẫn đến tệp ảnh

    [BsonElement("sStatus")]
    public string Status { get; set; }

    [BsonElement("iRedCard")]
    public int RedCard { get; set; }

    [BsonElement("iScrored")]
    public int Scrored { get; set; }
}

