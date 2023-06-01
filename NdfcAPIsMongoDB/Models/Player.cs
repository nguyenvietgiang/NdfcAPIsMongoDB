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
}

