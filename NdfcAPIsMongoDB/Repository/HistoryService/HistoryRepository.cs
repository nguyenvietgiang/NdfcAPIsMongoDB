using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.HistoryService
{
    public class HistoryRepository : IHistoryRepositorycs
    {
        private readonly IMongoCollection<ClubHistory> _historyCollection;

        public HistoryRepository(IMongoDatabase database)
        {
            _historyCollection = database.GetCollection<ClubHistory>("History");
        }

        public async Task<ClubHistory> GetClubHistoryAsync()
        {
            var clubHistory = await _historyCollection.Find(_ => true).FirstOrDefaultAsync();
            return clubHistory;
        }

        public async Task UpdateClubHistoryAsync(ClubHistory clubHistory)
        {
            var filter = Builders<ClubHistory>.Filter.Empty;
            await _historyCollection.ReplaceOneAsync(filter, clubHistory);
        }

        public async Task DeleteClubHistoryAsync()
        {
            var filter = Builders<ClubHistory>.Filter.Empty;
            await _historyCollection.DeleteOneAsync(filter);
        }
    }
}

