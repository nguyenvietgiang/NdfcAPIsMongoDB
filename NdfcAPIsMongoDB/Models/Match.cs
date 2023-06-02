﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NdfcAPIsMongoDB.Models
{
    public class Match
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("sEnemy")]
        public string Enemy { get; set; }

        [BsonElement("sStadium")]
        public string Stadium { get; set; }

        [BsonElement("dTime")]
        public string Time { get; set; }

        [BsonElement("sLeague")]
        public string League { get; set; }

        [BsonElement("iStatus")]
        public int Status { get; set; }
    }

}
