using MongoDB.Driver;
using Serilog.Core;
using Serilog.Events;

namespace NdfcAPIsMongoDB.Middleware
{
    public class MongoDBSink : ILogEventSink, IDisposable
    {
        private readonly IMongoCollection<LogEvent> _logCollection;

        public MongoDBSink(IMongoDatabase database, string collectionName)
        {
            _logCollection = database.GetCollection<LogEvent>(collectionName);
        }

        public void Emit(LogEvent logEvent)
        {
            _logCollection.InsertOne(logEvent);
        }

        public void Dispose()
        {

        }
    }
}

