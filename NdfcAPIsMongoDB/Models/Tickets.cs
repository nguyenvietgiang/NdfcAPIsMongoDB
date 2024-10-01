using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NdfcAPIsMongoDB.Models
{
    public class Tickets
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        // mã ghế
        public string SeatId { get; set; }
        // mã người đặt vé
        public string UserId { get; set; }
        public int price { get; set; }
        // trạng thái thanh toán
        public bool Status { get; set; }
    }
}
