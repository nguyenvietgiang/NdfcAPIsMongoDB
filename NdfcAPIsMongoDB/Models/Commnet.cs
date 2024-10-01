using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class Commnet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CommentId { get; set; }

        public string Content { get; set; }

        public string PostId { get; set; }

        public string UserId { get; set; }

        public string ParentCommentId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
