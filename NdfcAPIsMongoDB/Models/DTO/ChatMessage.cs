namespace NdfcAPIsMongoDB.Models.DTO
{
    public class ChatMessage
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime SentTime { get; set; }
    }
}
