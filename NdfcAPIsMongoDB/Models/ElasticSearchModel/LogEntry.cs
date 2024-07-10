namespace NdfcAPIsMongoDB.Models.ElasticSearchModel
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
    }

}
