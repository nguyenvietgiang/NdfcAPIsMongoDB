using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Common.PagingComon;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.SubscribService
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IMongoCollection<Subscriber> _subCollection;
        private readonly IPagingComon _pagingComon;
        public SubscriberRepository(IMongoDatabase database ,IPagingComon pagingComon)
        {
            _subCollection = database.GetCollection<Subscriber>("Subscriber");
            _pagingComon = pagingComon;
        }
        public async Task<Respaging<Subscriber>> GetAllSubcriber(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            return await _pagingComon.GetAllData<Subscriber>(pageNumber, pageSize, searchName);
        }

        public async Task<Subscriber> Subcribe(Subscriber subscriber)
        {
            await _subCollection.InsertOneAsync(subscriber);
            return subscriber;
        }
    }
}
