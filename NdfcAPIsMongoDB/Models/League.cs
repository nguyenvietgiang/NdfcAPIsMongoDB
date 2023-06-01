namespace NdfcAPIsMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class League
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("sName")]
    public string Name { get; set; }

    [BsonElement("sReward")]
    public string Reward { get; set; }

    [BsonElement("sYear")]
    public string Year { get; set; }

    [BsonElement("iStatus")]
    public int Status { get; set; }
}

